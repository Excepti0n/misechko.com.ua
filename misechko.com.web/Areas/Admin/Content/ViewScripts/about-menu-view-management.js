function AboutMenuModel(pubData, parent) {
    var self = this;

    self.Id = pubData.Id;
    self.AboutMenuName = pubData.Headline;
    self.AboutMenuPath = pubData.LinkPath;
    self.AboutMenuHREF = '/Read' + self.AboutMenuPath;
    self.DateCreated = ko.observable(pubData.PublishDate);
    self.Type = pubData.Type;

    self.ItemChanged = ko.observable(false);
    
    self.IndexChanged = ko.observable(false);

    self.ItemIndex = ko.observable(pubData.Index);
    self.ItemIndex.subscribe(function () {
        self.ItemChanged(true);
    });

    self.DateCreated.subscribe(function () {
        self.ItemChanged(true);
    });


    self.Remove = function () {
        var removePubUrl = $('#RemoveAboutMenuUrl').val();
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
                    alert("There was an error removing the aboutMenu - " + res);
                }
            }
        });
    };

    self.UpdateAboutMenu = function () {
        var updatePubUrl = $('#UpdateAboutMenuUrl').val();
        $.ajax({
            type: 'POST',
            url: updatePubUrl,
            data: {
                id: self.Id,
                aboutMenuName: self.AboutMenuName,
                dateCreated: self.DateCreated(),
                index: self.ItemIndex()
            },
            success: function (res) {
                if (res.status === "SPCD: OK") {
                    self.ItemChanged(false);
                } else {
                    alert("There was an error updating the aboutMenu - " + res.status);
                }
            }
        });
    };
}

function AboutMenuManagementViewModel(initData) {
    var self = this;

    self.RemovePub = function (pubVM) {
        self.AboutMenus.remove(pubVM);
    };

    self.NewAboutMenuName = ko.observable('').extend({ required: true });

    self.AboutMenus = ko.observableArray([]);

    var aboutMenusArray = jQuery.map(initData.AboutMenus, function (val, i) {
        self.AboutMenus.push(new AboutMenuModel(val, self, i == 0));
    });

    self.AddNewAboutMenu = function () {
        var addAboutMenuUrl = $('#AddAboutMenuUrl').val();
        $.ajax({
            type: 'POST',
            url: addAboutMenuUrl,
            data: {
                aboutMenuName: self.NewAboutMenuName()
            },
            success: function (res) {
                if (res.status === "SPCD: AMADDED") {
                    var pbVM = new AboutMenuModel(res.aboutMenu, self, false);
                    self.AboutMenus.push(pbVM);
                    self.NewAboutMenuName('');
                } else {
                    alert("There was an error adding the aboutMenu: " + res);
                }
            }
        });
    };
}

var AboutMenuView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialAboutMenuDataObject = $.parseJSON($('#initial-aboutMenu-list').val());
        var vm = new AboutMenuManagementViewModel(initialAboutMenuDataObject);

        ko.applyBindings(vm, document.getElementById("aboutMenu-management-view"));
    }
};