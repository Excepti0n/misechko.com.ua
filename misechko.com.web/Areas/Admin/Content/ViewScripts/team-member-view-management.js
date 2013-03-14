function TeamMemberModel(pubData, parent) {
    var self = this;

    self.Id = pubData.Id;
    self.TeamMemberName = pubData.Headline;
    self.TeamMemberPath = pubData.LinkPath;
    self.TeamMemberHREF = '/Read' + self.TeamMemberPath;
    self.DateCreated = ko.observable(pubData.PublishDate);
    self.Type = pubData.Type;

    self.ItemChanged = ko.observable(false);

    self.DateCreated.subscribe(function () {
        self.ItemChanged(true);
    });
    
    self.IndexChanged = ko.observable(false);

    self.ItemIndex = ko.observable(pubData.Index);
    self.ItemIndex.subscribe(function () {
        self.ItemChanged(true);
    });


    self.Remove = function () {
        var removePubUrl = $('#RemoveTeamMemberUrl').val();
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
                    alert("There was an error removing the teamMember - " + res);
                }
            }
        });
    };

    self.UpdateTeamMember = function () {
        var updatePubUrl = $('#UpdateTeamMemberUrl').val();
        $.ajax({
            type: 'POST',
            url: updatePubUrl,
            data: {
                id: self.Id,
                teamMemberName: self.TeamMemberName,
                dateCreated: self.DateCreated(),
                index: self.ItemIndex()
            },
            success: function (res) {
                if (res.status === "SPCD: OK") {
                    self.ItemChanged(false);
                } else {
                    alert("There was an error updating the teamMember - " + res.status);
                }
            }
        });
    };
}

function TeamMemberManagementViewModel(initData) {
    var self = this;

    self.RemovePub = function (pubVM) {
        self.TeamMembers.remove(pubVM);
    };

    self.NewTeamMemberName = ko.observable('').extend({ required: true });

    self.TeamMembers = ko.observableArray([]);

    var teamMembersArray = jQuery.map(initData.TeamMembers, function (val, i) {
        self.TeamMembers.push(new TeamMemberModel(val, self, i == 0));
    });

    self.AddNewTeamMember = function () {
        var addTeamMemberUrl = $('#AddTeamMemberUrl').val();
        $.ajax({
            type: 'POST',
            url: addTeamMemberUrl,
            data: {
                teamMemberName: self.NewTeamMemberName()
            },
            success: function (res) {
                if (res.status === "SPCD: TMADDED") {
                    var pbVM = new TeamMemberModel(res.teamMember, self, false);
                    self.TeamMembers.push(pbVM);
                    self.NewTeamMemberName('');
                } else {
                    alert("There was an error adding the teamMember: " + res);
                }
            }
        });
    };
}

var TeamMemberView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialTeamMemberDataObject = $.parseJSON($('#initial-teamMember-list').val());
        var vm = new TeamMemberManagementViewModel(initialTeamMemberDataObject);

        ko.applyBindings(vm, document.getElementById("teamMember-management-view"));
    }
};