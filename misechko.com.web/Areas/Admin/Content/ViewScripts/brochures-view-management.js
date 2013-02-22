function BrochureModel(pubData, parent) {
    var self = this;

    self.Id = pubData.Id;
    self.BrochureName = pubData.Headline;
    self.BrochurePath = pubData.LinkPath;
    self.BrochureHREF = '/Read' + self.BrochurePath;
    self.DateCreated = pubData.PublishDate;
    self.Type = pubData.Type;

    
    self.Remove = function () {
        var removePubUrl = $('#RemoveBrochureUrl').val();
        $.ajax({
            type: 'POST',
            url: removePubUrl,
            data: {
                id: self.Id
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemovePub(self);
                } else {
                    alert("There was an error removing the brochure - " + res);
                }
            }
        });
    };
    
    self.UpdateBrochure = function () {
        var updatePubUrl = $('#UpdateBrochureUrl').val();
        $.ajax({
            type: 'POST',
            url: updatePubUrl,
            data: {
                id: self.Id,
                brochureName: self.BrochureName,
                dateCreated: self.DateCreated
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemovePub(self);
                } else {
                    alert("There was an error removing the brochure - " + res);
                }
            }
        });
    };
}

function BrochuresManagementViewModel(initData) {
    var self = this;

    self.RemovePub = function (pubVM) {
        self.Brochures.remove(pubVM);
    };

    self.NewBrochureName = ko.observable('').extend({ required: true });
    
    self.Brochures = ko.observableArray([]);

    var brochuresArray = jQuery.map(initData.Brochures, function (val, i) {
        self.Brochures.push(new BrochureModel(val, self, i == 0));
    });

    self.AddNewBrochure = function () {
        var addBrochureUrl = $('#AddBrochureUrl').val();
        $.ajax({
            type: 'POST',
            url: addBrochureUrl,
            data: {
                brochureName: self.NewBrochureName()
            },
            success: function (res) {
                if (res.status === "SPCD: BRADDED") {
                    var pbVM = new BrochureModel(res.brochure, self, false);
                    self.Brochures.push(pbVM);
                    self.NewBrochureName('');
                } else {
                    alert("There was an error adding the brochure: " + res.status);
                }
            }
        });
    };
}

var BrochuresView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialBrochuresDataObject = $.parseJSON($('#initial-brochures-list').val());
        var vm = new BrochuresManagementViewModel(initialBrochuresDataObject);

        ko.applyBindings(vm, document.getElementById("brochures-management-view"));
    }
};