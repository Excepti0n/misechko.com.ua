$(function() {

    /* USAGE EXAMPLE FOR jqAutocomple binding

    <input data-bind="jqAuto: { autoFocus: true },  
                jqAutoSource: CodesContainer,
                jqAutoQuery: GetCodes, 
                jqAutoValue: CurrentCode" />

                */

    //jqAuto -- main binding (should contain additional options to pass to autocomplete)
    //jqAutoSource -- the array to populate with choices (needs to be an observableArray)
    //jqAutoQuery -- function to return choices
    //jqAutoValue -- where to write the selected value
    //jqAutoSourceLabel -- the property that should be displayed in the possible choices
    //jqAutoSourceInputValue -- the property that should be displayed in the input box
    //jqAutoSourceValue -- the property to use for the value
    ko.bindingHandlers.jqAuto = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var options = valueAccessor() || {},
                allBindings = allBindingsAccessor(),
                unwrap = ko.utils.unwrapObservable,
                modelValue = allBindings.jqAutoValue,
                source = allBindings.jqAutoSource,
                query = allBindings.jqAutoQuery,
                valueProp = allBindings.jqAutoSourceValue,
                inputValueProp = allBindings.jqAutoSourceInputValue || valueProp,
                labelProp = allBindings.jqAutoSourceLabel || inputValueProp;

            //function that is shared by both select and change event handlers
            function writeValueToModel(valueToWrite) {
                if (ko.isWriteableObservable(modelValue)) {
                    modelValue(valueToWrite);
                } else {  //write to non-observable
                    if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['jqAutoValue'])
                        allBindings['_ko_property_writers']['jqAutoValue'](valueToWrite);
                }
            }

            //on a selection write the proper value to the model
            options.select = function (event, ui) {
                writeValueToModel(ui.item ? ui.item.actualValue : null);
            };

            //on a change, make sure that it is a valid value or clear out the model value
            options.change = function (event, ui) {
                var currentValue = $(element).val();
                var matchingItem = ko.utils.arrayFirst(unwrap(source), function (item) {
                    return unwrap(inputValueProp ? item[inputValueProp] : item) === currentValue;
                });

                if (!matchingItem) {
                    writeValueToModel(null);
                }
            }

            //hold the autocomplete current response
            var currentResponse = null;

            //handle the choices being updated in a DO, to decouple value updates from source (options) updates
            var mappedSource = ko.dependentObservable({
                read: function () {
                    var mapped = ko.utils.arrayMap(unwrap(source), function (item) {
                        var result = {};
                        result.label = labelProp ? unwrap(item[labelProp]) : unwrap(item).toString();  //show in pop-up choices
                        result.value = inputValueProp ? unwrap(item[inputValueProp]) : unwrap(item).toString();  //show in input box
                        result.actualValue = valueProp ? unwrap(item[valueProp]) : item;  //store in model
                        return result;
                    });
                    return mapped;
                },
                write: function (newValue) {
                    source(newValue);  //update the source observableArray, so our mapped value (above) is correct
                    if (currentResponse) {
                        currentResponse(mappedSource());
                    }
                },
                disposeWhenNodeIsRemoved: element
            });

            if (query) {
                options.source = function (request, response) {
                    currentResponse = response;
                    query.call(this, request.term, mappedSource);
                }
            } else {
                //whenever the items that make up the source are updated, make sure that autocomplete knows it
                mappedSource.subscribe(function (newValue) {
                    $(element).autocomplete("option", "source", newValue);
                });

                options.source = mappedSource();
            }


            //initialize autocomplete
            $(element).autocomplete(options);
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            //update value based on a model change
            var allBindings = allBindingsAccessor(),
                unwrap = ko.utils.unwrapObservable,
                modelValue = unwrap(allBindings.jqAutoValue) || '',
                valueProp = allBindings.jqAutoSourceValue,
                inputValueProp = allBindings.jqAutoSourceInputValue || valueProp;

            //if we are writing a different property to the input than we are writing to the model, then locate the object
            if (valueProp && inputValueProp !== valueProp) {
                var source = unwrap(allBindings.jqAutoSource) || [];
                var modelValue = ko.utils.arrayFirst(source, function (item) {
                    return unwrap(item[valueProp]) === modelValue;
                }) || {};
            }

            //update the element with the value that should be shown in the input
            $(element).val(modelValue && inputValueProp !== valueProp ? unwrap(modelValue[inputValueProp]) : modelValue.toString());
        }
    };

    ko.bindingHandlers.timeago = {
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            var $this = $(element);

            // Set the title attribute to the new value = timestamp
            $this.attr('title', value);

            // If timeago has already been applied to this node, don't reapply it -
            // since timeago isn't really flexible (it doesn't provide a public
            // remove() or refresh() method) we need to do everything by ourselves.
            if ($this.data('timeago')) {
                var datetime = $.timeago.datetime($this);
                var distance = (new Date().getTime() - datetime.getTime());
                var inWords = $.timeago.inWords(distance);

                // Update cache and displayed text..
                $this.data('timeago', { 'datetime': datetime });
                $this.text(inWords);
            } else {
                // timeago hasn't been applied to this node -> we do that now!
                $this.timeago();
            }
        }
    };

    ko.bindingHandlers.chosen = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var options = ko.utils.unwrapObservable(valueAccessor()) || {};
            var allBindings = allBindingsAccessor();
            var attrList = { placeholderText: options.PlaceholderText };
            $.extend(attrList, allBindings.chosen);
            if (options.AddPlaceholder) {
                $(element).attr('data-placeholder', attrList.placeholderText).addClass('chzn-select');
            } else {
                $(element).addClass('chzn-select');
            }
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var options = ko.utils.unwrapObservable(valueAccessor()) || {};
            $(element).chosen(options.ChosenOptions);
            $(element).trigger("liszt:updated");
        }
    };
    
    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            //initialize datepicker with some optional options
            var options = allBindingsAccessor().datepickerOptions || {};
            $(element).datepicker(options);

            //handle the field changing
            ko.utils.registerEventHandler(element, "change", function () {
                var observable = valueAccessor();
                var selectedDate = $(element).datepicker("getDate");
                var formattedResult = $.datepicker.formatDate(options.dateFormat, $(this).datepicker("getDate"));
                observable(formattedResult);
            });

            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).datepicker("destroy");
            });

        },
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor()),
                current = $(element).datepicker("getDate");

            //if (value - current !== 0) {
                $(element).datepicker("setDate", value);
            //}
        }
    };
    
    ko.bindingHandlers.jQRangePicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = ko.utils.unwrapObservable(valueAccessor()) || {};
            //do in a setTimeout, so the applyBindings doesn't bind twice from element being copied and moved to bottom
            setTimeout(function () {

                $(element).rangeSlider(options);
                $(element).bind("valuesChanged", function (event, data) {
                    if (isNaN(data.values.max) || isNaN(data.values.min)) return;
                    var value = allBindingsAccessor().value;
                    value(options.resultInterpreter(data.values));
                });
                
                if(options.prohibitLeftChange) {
                    $(element).bind("valuesChanging", function (event, data) {
                        if (options.leftFix != data.values.min) {
                            $(element).rangeSlider("min", options.leftFix);
                        }
                    });
                }
            }, 0);
        }
    };
    
    ko.bindingHandlers.tagsinput = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = ko.utils.unwrapObservable(valueAccessor()) || {};
            //do in a setTimeout, so the applyBindings doesn't bind twice from element being copied and moved to bottom
            setTimeout(function () {
                options.onChange = function (obj, tagslist) {
                    var value = allBindingsAccessor().value;
                    var arrayOfTags = obj.val().split(',');
                    value(arrayOfTags);
                };

                $(element).tagsInput(options);

                if (options.defaultTags && options.defaultTags()) {
                    $(element).importTags(options.defaultTags().join(","));
                }
                
            }, 0);

        }
    };

    ko.bindingHandlers.dialog = {
        init: function(element, valueAccessor, allBindingsAccessor) {
            var options = ko.utils.unwrapObservable(valueAccessor()) || { };
            //do in a setTimeout, so the applyBindings doesn't bind twice from element being copied and moved to bottom
            setTimeout(function() {
                options.close = function() {
                    allBindingsAccessor().dialogVisible(false);
                };

                $(element).dialog(options);
            }, 0);

            //handle disposal (not strictly necessary in this scenario)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                $(element).dialog("destroy");
            });
        },
        update: function(element, valueAccessor, allBindingsAccessor) {
            var shouldBeOpen = ko.utils.unwrapObservable(allBindingsAccessor().dialogVisible);
            $(element).dialog(shouldBeOpen ? "open" : "close");
        }
    };
    
    ko.bindingHandlers.ckeditor = {
        init: function (element, valueAccessor, allBindingsAccessor, context) {
            var options = allBindingsAccessor().ckeditorOptions || {};
            var modelValue = valueAccessor();
            var value = ko.utils.unwrapObservable(valueAccessor());

            $(element).html(value);
            $(element).ckeditor();

            var editor = $(element).ckeditorGet();

            editor.on('blur', function (e) {
                var self = this;
                if (ko.isWriteableObservable(self)) {
                    self($(e.listenerData).val());
                }
            }, modelValue, element);

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                var existingEditor = CKEDITOR.instances[element.name];
                existingEditor.destroy(true);
            });
        }
    };
    
    ko.bindingHandlers.executeOnEnter = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var allBindings = allBindingsAccessor();
            $(element).keypress(function (event) {
                var keyCode = (event.which ? event.which : event.keyCode);
                if (keyCode === 13) {
                    allBindings.executeOnEnter.call(viewModel);
                    return false;
                }
                return true;
            });
        }
    };
    
    ko.bindingHandlers.fadeVisible = {
        init: function (element, valueAccessor) {
            // Initially set the element to be instantly visible/hidden depending on the value
            var value = valueAccessor();
            $(element).toggle(ko.utils.unwrapObservable(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
        },
        update: function (element, valueAccessor) {
            // Whenever the value subsequently changes, slowly fade the element in or out
            var value = valueAccessor();
            ko.utils.unwrapObservable(value) ? $(element).fadeIn() : $(element).fadeOut();
        }
    };
    
    ko.bindingHandlers.stopBinding = {
        init: function () {
            return { controlsDescendantBindings: true };
        }
    };
});