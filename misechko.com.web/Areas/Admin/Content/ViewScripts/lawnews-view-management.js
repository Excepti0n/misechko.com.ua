function LawNewsItemModel(pubData, parent) {
    var self = this;

    self.Id = pubData.Id;
    self.LawNewsItemName = pubData.Headline;
    self.LawNewsItemPath = pubData.LinkPath;
    self.LawNewsItemHREF = '/Read' + self.LawNewsItemPath;
    self.DateCreated = ko.observable(pubData.PublishDate);
    self.Type = pubData.Type;

    self.ItemChanged = ko.observable(false);

    self.DateCreated.subscribe(function () {
        self.ItemChanged(true);
    });

    
    self.Remove = function () {
        var removeNIUrl = $('#RemoveLawNewsItemUrl').val();
        $.ajax({
            type: 'POST',
            url: removeNIUrl,
            data: {
                id: self.Id
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemoveNI(self);
                } else {
                    alert("There was an error removing the news item - " + res);
                }
            }
        });
    };
    
    self.UpdateLawNewsItem = function () {
        var updateNIUrl = $('#UpdateLawNewsItemUrl').val();
        $.ajax({
            type: 'POST',
            url: updateNIUrl,
            data: {
                id: self.Id,
                newsItemName: self.LawNewsItemName,
                dateCreated: self.DateCreated
            },
            success: function (res) {
                if (res.status === "SPCD: OK") {
                    self.ItemChanged(false);
                } else {
                    alert("There was an error updating the newsItem - " + res.status);
                }
            }
        });
    };
}

function LawNewsItemsManagementViewModel(initData) {
    var self = this;

    self.RemoveNI = function (pubVM) {
        self.LawNewsItems.remove(pubVM);
    };

    self.NewLawNewsItemName = ko.observable('').extend({ required: true });
    
    self.LawNewsItems = ko.observableArray([]);

    var newsArray = jQuery.map(initData.LawNews, function (val, i) {
        self.LawNewsItems.push(new LawNewsItemModel(val, self, i == 0));
    });

    self.AddNewLawNewsItem = function () {
        var addLawNewsItemUrl = $('#AddLawNewsItemUrl').val();
        $.ajax({
            type: 'POST',
            url: addLawNewsItemUrl,
            data: {
                lawNewsItemName: self.NewLawNewsItemName()
            },
            success: function (res) {
                if (res.status === "SPCD: LNIADDED") {
                    var pbVM = new LawNewsItemModel(res.newsItem, self, false);
                    self.LawNewsItems.push(pbVM);
                    self.NewLawNewsItemName('');
                } else {
                    alert("There was an error adding the news item: " + res.status);
                }
            }
        });
    };
}

var LawNewsView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialLawNewsItemsDataObject = $.parseJSON($('#initial-LawNews-list').val());
        var vm = new LawNewsItemsManagementViewModel(initialLawNewsItemsDataObject);

        ko.applyBindings(vm, document.getElementById("LawNews-management-view"));
    }
};