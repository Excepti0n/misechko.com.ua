function NewsItemModel(pubData, parent) {
    var self = this;

    self.Id = pubData.Id;
    self.NewsItemName = pubData.Headline;
    self.NewsItemPath = pubData.LinkPath;
    self.NewsItemHREF = '/Read' + self.NewsItemPath;
    self.DateCreated = pubData.PublishDate;
    self.Type = pubData.Type;

    
    self.Remove = function () {
        var removeNIUrl = $('#RemoveNewsItemUrl').val();
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
                    alert("There was an error removing the newsItem - " + res);
                }
            }
        });
    };
    
    self.UpdateNewsItem = function () {
        var updateNIUrl = $('#UpdateNewsItemUrl').val();
        $.ajax({
            type: 'POST',
            url: updateNIUrl,
            data: {
                id: self.Id,
                newsItemName: self.NewsItemName,
                dateCreated: self.DateCreated
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemoveNI(self);
                } else {
                    alert("There was an error removing the newsItem - " + res);
                }
            }
        });
    };
}

function NewsItemsManagementViewModel(initData) {
    var self = this;

    self.RemoveNI = function (pubVM) {
        self.NewsItems.remove(pubVM);
    };

    self.NewNewsItemName = ko.observable('').extend({ required: true });
    
    self.NewsItems = ko.observableArray([]);

    var newsArray = jQuery.map(initData.News, function (val, i) {
        self.NewsItems.push(new NewsItemModel(val, self, i == 0));
    });

    self.AddNewNewsItem = function () {
        var addNewsItemUrl = $('#AddNewsItemUrl').val();
        $.ajax({
            type: 'POST',
            url: addNewsItemUrl,
            data: {
                newsItemName: self.NewNewsItemName()
            },
            success: function (res) {
                if (res.status === "SPCD: NIADDED") {
                    var pbVM = new NewsItemModel(res.newsItem, self, false);
                    self.NewsItems.push(pbVM);
                    self.NewNewsItemName('');
                } else {
                    alert("There was an error adding the news item: " + res.status);
                }
            }
        });
    };
}

var NewsView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialNewsItemsDataObject = $.parseJSON($('#initial-news-list').val());
        var vm = new NewsItemsManagementViewModel(initialNewsItemsDataObject);

        ko.applyBindings(vm, document.getElementById("news-management-view"));
    }
};