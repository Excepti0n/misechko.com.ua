$(function () {

    function PartyToEditViewModel(party, parent) {
        var self = this;

        self.Name = ko.computed({
            read: function() {
                return party.Name();
            },
            write: function(value) {
                party.Name(value);
                self.HasUnsavedChanges(true);
            }
        });

        self.CreatePartyProfile = function() {
            $.ajaxSetup({
                url: $('#CreatePartyProfileUrl').val(),
                global: false,
                type: "GET",
                data: { id: party.Id }
            });

            $.ajax().done(function (result) {
                if (result === "SPCD: PROFILECREATED") {
                    self.HasDescr(true);
                }
            });
        };

        self.HasDescr = ko.observable(false);
        self.DescrStore = ko.observable();

        self.Descr = ko.computed({
            read: function () {
                if (!self.DescrStore()) {
                    $.ajaxSetup({
                        url: $('#GetDescrUrl').val(),
                        global: false,
                        type: "GET",
                        data: { id: party.Id },
                        async: false
                    });

                    var res = ko.observable();

                    $.ajax().done(function(result) {
                        if (result !== "SPCD: NO PROFILE") {
                            self.HasDescr(true);
                            res(result);
                        }
                    });

                    return res();
                } else {
                    return self.DescrStore();
                }
            },
            write: function (value) {
                self.HasUnsavedChanges(true);
                self.DescrStore(value);
            }
        });

        self.HasUnsavedChanges = ko.observable(false);

        self.SaveChangesText = ko.observable('Сохранить изменения');
        self.IsSavingPartyInfo = ko.observable(false);

        self.SaveEdits = function () {
            if (!self.IsSavingPartyInfo()) {
                self.IsSavingPartyInfo(true);
                $.ajax({
                    url: $('#UpdatePartyInfoUrl').val(),
                    global: false,
                    type: "POST",
                    data: {
                        id: party.Id,
                        name: party.Name(),
                        generalInfo: self.DescrStore()
                    },
                    beforeSend: function() {
                        self.SaveChangesText('Сохраняю...');
                    }
                        
                }).done(function (resCode) {
                    self.HasUnsavedChanges(false);
                    self.SaveChangesText('Сохранить изменения');
                    self.IsSavingPartyInfo = ko.observable(false);
                });
            }
        };

        self.DeputiesShown = ko.observable(false);

        self.ListPartyDeputies = function() {
            if (!self.DeputiesShown()) {
                $.ajaxSetup({
                    url: $('#GetDeputiesForPartyUrl').val(),
                    global: false,
                    type: "GET",
                    data: { id: party.Id },
                    async: false
                });

                var vmData;

                $.ajax().done(function(model) {
                    vmData = model;
                });

                $.ajaxSetup({
                    url: $('#GetDeputiesManagementViewUrl').val(),
                    global: false,
                    type: "GET",
                    async: false
                });

                var view;

                $.ajax().done(function(v) {
                    view = $(v);
                });

                $('#party-edit-block').append(view);

                DeputiesManagement.Init(vmData);

                self.DeputiesShown(true);
            }
        };
    }

    function PartyViewModel(id, name, parent) {
        var self = this;
        self.Id = id;
        self.Name = ko.observable(name);
        self.Selected = ko.observable(false);

        self.OpenPartyForEdit = function() {
            parent.OpenPartyEdit(self);
            parent.DeselectOthers();
            self.Selected(true);
        };
    }

    function PartiesManagementViewModel(initialData) {
        var self = this;

        var parties = $.makeArray(initialData.Parties);

        self.PartyEditBlockLoaded = false;

        var partiesModelsArray = jQuery.map(parties, function (val, i) {
            return (new PartyViewModel(val.Id, val.Name, self));
        });

        self.Parties = ko.observableArray(partiesModelsArray);

        self.PartyToEdit = ko.observable();

        self.PartyEditBlockVisible = ko.observable(false);

        self.DeselectOthers = function() {
            ko.utils.arrayForEach(self.Parties(), function (partyModel) {
                partyModel.Selected(false);
            });
        };

        self.OpenPartyEdit = function (party) {
            
            if(!self.PartyEditBlockLoaded) {

                $.ajaxSetup({
                    url: $('#GetPartyEditViewUrl').val(),
                    global: false,
                    type: "GET",
                    async: false
                });

                var view;

                $.ajax().done(function (v) {
                    view = $(v);
                });

                $('#party-edit-block-wrapper').append(view);
                self.PartyEditBlockLoaded = true;
            }

            if (self.PartyToEdit()) {
                if (self.PartyToEdit().DeputiesShown()) {
                    ko.cleanNode($('#deputies-management-view')[0]);
                    $('#deputies-management-view').remove();
                }
            }

            var editVM = new PartyToEditViewModel(party, self);
            self.PartyToEdit(editVM);
            ko.cleanNode($('#party-edit-block')[0]);
            ko.applyBindings(editVM, document.getElementById("party-edit-block"));
            self.PartyEditBlockVisible(true);

        };
    }

   var viewVM = new PartiesManagementViewModel($.parseJSON($('#initial-parties-list').val()));

   ko.applyBindings(viewVM, document.getElementById("parties-management-view"));
});