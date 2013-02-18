function PublicationModel(pubData, parent) {
    var self = this;

    self.Id = pubData.Id;
    self.PublicationName = pubData.Headline;
    self.PublicationPath = pubData.LinkPath
    self.DateCreated = pubData.PublishDate;
    
    self.Remove = function () {
        var removePubUrl = $('#DeletePublicationUrl').val();
        $.ajax({
            type: 'POST',
            url: removePubUrl,
            data: {
                userName: self.Id
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemovePub(self);
                } else {
                    alert("There was an error removing the publication - " + res);
                }
            }
        });
    };
    
    self.UpdatePublication = function () {
        var updatePubUrl = $('#UpdatePublicationUrl').val();
        $.ajax({
            type: 'POST',
            url: updatePubUrl,
            data: {
                id: self.Id,
                publicationName: self.PublicationName,
                dateCreated: self.DateCreated
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemovePub(self);
                } else {
                    alert("There was an error removing the publication - " + res);
                }
            }
        });
    };
}

function PublicationsManagementViewModel(initData) {
    var self = this;

    self.RemovePub = function (pubVM) {
        self.Publications.remove(pubVM);
    };

    self.NewPublicationName = ko.observable('').extend({ required: true });
    
    self.Publications = ko.observableArray([]);

    var publicationsArray = jQuery.map(initData.Publications, function (val, i) {
        self.Publications.push(new PublicationModel(val, self, i == 0));
    });

    self.AddNewPublication = function () {
        var addPublicationUrl = $('#AddPublicationUrl').val();
        $.ajax({
            type: 'POST',
            url: addPublicationUrl,
            data: {
                publicationName: self.NewPublicationName()
            },
            success: function (res) {
                if (res.status === "SPCD: PBADDED") {
                    var pbVM = new PublicationModel(res.pub, self, false);
                    self.Publications.push(pbVM);
                    self.NewPublicationName('');
                } else {
                    alert("There was an error adding the publication: " + res);
                }
            }
        });
    };
}

var PublicationsView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialPublicationsDataObject = $.parseJSON($('#initial-publications-list').val());
        var vm = new PublicationsManagementViewModel(initialPublicationsDataObject);

        ko.applyBindings(vm, document.getElementById("publications-management-view"));
    }
};