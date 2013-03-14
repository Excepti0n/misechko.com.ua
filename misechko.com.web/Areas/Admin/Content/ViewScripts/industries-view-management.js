function IndustryModel(industryData, parent) {
    var self = this;

    self.Id = industryData.Id;
    self.IndustryName = industryData.Headline;
    self.IndustryPath = industryData.LinkPath;
    self.IndustryHREF = '/Read' + self.IndustryPath;
    self.DateCreated = industryData.PublishDate;
    self.Type = industryData.Type;

    self.IndexChanged = ko.observable(false);

    self.ItemIndex = ko.observable(industryData.Index);
    self.ItemIndex.subscribe(function () {
        self.IndexChanged(true);
    });

    self.IndustryProjectsChanged = ko.observable(false);
    self.IndustryPublicationsChanged = ko.observable(false);

    self.IndustryProjects = ko.observableArray(industryData.Projects);
    self.IndustryProjects.subscribe(function () {
        self.IndustryProjectsChanged(true);
    });
    self.IndustryPublications = ko.observableArray(industryData.Publications);
    self.IndustryPublications.subscribe(function () {
        self.IndustryPublicationsChanged(true);
    });

    self.ParentProjects = ko.computed(function () {
        return jQuery.map(parent.AllProjects(), function (val, i) {
            return val.ProjectName;
        });
    });
    
    self.ParentPublications = ko.computed(function () {
        return jQuery.map(parent.AllPublications(), function (val, i) {
            return val.PublicationName;
        });
    });
    
    self.Remove = function () {
        var removePubUrl = $('#RemoveIndustryUrl').val();
        $.ajax({
            type: 'POST',
            url: removePubUrl,
            data: {
                id: self.Id
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemoveIndustry(self);
                } else {
                    alert("There was an error removing the Industry - " + res);
                }
            }
        });
    };
    
    self.UpdateIndustry = function () {
        var updatePubUrl = $('#UpdateIndustryUrl').val();
        $.ajax({
            type: 'POST',
            url: updatePubUrl,
            data: {
                id: self.Id,
                IndustryName: self.IndustryName,
                projectsInIndustry: JSON.stringify(self.IndustryProjects()),
                publicationsInIndustry: JSON.stringify(self.IndustryPublications()),
                dateCreated: self.DateCreated,
                index: self.ItemIndex()
            },
            success: function (res) {
                if (res.status === "SPCD: OK") {
                    self.IndustryPublicationsChanged(false);
                    self.IndustryProjectsChanged(false);
                } else {
                    alert("There was an error updating the Industry - " + res.status);
                }
            }
        });
    };
}

function PorjectModel(modelData, parent) {
    var self = this;

    self.ProjectName = modelData.Name;
}

function PublicationModel(modelData, parent) {
    var self = this;

    self.PublicationName = modelData.Name;
}

function IndustriesManagementViewModel(initData) {
    var self = this;

    self.RemoveIndustry = function (industryVM) {
        self.Industries.remove(industryVM);
    };

    self.NewIndustryName = ko.observable('').extend({ required: true });
    
    self.AllProjects = ko.observableArray([]);

    var ProjectsArray = jQuery.map(initData.AllProjects, function (val, i) {
        self.AllProjects.push(new ProjectModel(val, self));
    });

    self.AllPublications = ko.observableArray([]);

    var PublicationsArray = jQuery.map(initData.AllPublications, function (val, i) {
        self.AllPublications.push(new PublicationModel(val, self));
    });
    
    self.Industries = ko.observableArray([]);

    var IndustriesArray = jQuery.map(initData.Industries, function (val, i) {
        self.Industries.push(new IndustryModel(val, self, i == 0));
    });
    
    self.AddNewIndustry = function () {
        var addIndustryUrl = $('#AddIndustryUrl').val();
        $.ajax({
            type: 'POST',
            url: addIndustryUrl,
            data: {
                IndustryName: self.NewIndustryName()
            },
            success: function (res) {
                if (res.status === "SPCD: INADDED") {
                    var pbVM = new IndustryModel(res.industry, self, false);
                    self.Industries.push(pbVM);
                    self.NewIndustryName('');
                } else {
                    alert("There was an error adding the Industry: " + res.status);
                }
            }
        });
    };
}

var IndustriesView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialIndustriesDataObject = $.parseJSON($('#initial-industries-list').val());
        var vm = new IndustriesManagementViewModel(initialIndustriesDataObject);

        ko.applyBindings(vm, document.getElementById("industries-management-view"));
    }
};