function AwardModel(pubData, parent) {
    var self = this;

    self.Id = pubData.Id;
    self.AwardName = pubData.Headline;
    self.AwardPath = pubData.LinkPath;
    self.AwardHREF = '/Read' + self.AwardPath;
    self.DateCreated = pubData.PublishDate;
    self.Type = pubData.Type;

    
    self.Remove = function () {
        var removePubUrl = $('#RemoveAwardUrl').val();
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
                    alert("There was an error removing the award - " + res);
                }
            }
        });
    };
    
    self.UpdateAward = function () {
        var updatePubUrl = $('#UpdateAwardUrl').val();
        $.ajax({
            type: 'POST',
            url: updatePubUrl,
            data: {
                id: self.Id,
                awardName: self.AwardName,
                dateCreated: self.DateCreated
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemovePub(self);
                } else {
                    alert("There was an error removing the award - " + res);
                }
            }
        });
    };
}

function AwardsManagementViewModel(initData) {
    var self = this;

    self.RemovePub = function (pubVM) {
        self.Awards.remove(pubVM);
    };

    self.NewAwardName = ko.observable('').extend({ required: true });
    
    self.Awards = ko.observableArray([]);

    var awardsArray = jQuery.map(initData.Awards, function (val, i) {
        self.Awards.push(new AwardModel(val, self, i == 0));
    });

    self.AddNewAward = function () {
        var addAwardUrl = $('#AddAwardUrl').val();
        $.ajax({
            type: 'POST',
            url: addAwardUrl,
            data: {
                awardName: self.NewAwardName()
            },
            success: function (res) {
                if (res.status === "SPCD: AWADDED") {
                    var pbVM = new AwardModel(res.pub, self, false);
                    self.Awards.push(pbVM);
                    self.NewAwardName('');
                } else {
                    alert("There was an error adding the award: " + res);
                }
            }
        });
    };
}

var AwardsView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialAwardsDataObject = $.parseJSON($('#initial-awards-list').val());
        var vm = new AwardsManagementViewModel(initialAwardsDataObject);

        ko.applyBindings(vm, document.getElementById("awards-management-view"));
    }
};