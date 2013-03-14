function IndustryModel(industryData, parent) {
    var self = this;

    self.Id = industryData.Id;
    self.Name = industryData.Headline;
}

function PracticeModel(practiceData, parent) {
    var self = this;

    self.Id = practiceData.Id;
    self.Name = practiceData.Headline;
}

function ProjectModel(pubData, parent) {
    var self = this;

    self.Id = pubData.Id;
    self.ProjectName = pubData.Headline;
    self.ProjectPath = pubData.LinkPath;
    self.ProjectHREF = '/Read' + self.ProjectPath;
    self.DateCreated = ko.observable(pubData.PublishDate);
    self.Type = pubData.Type;
}

function ProjectsSearchViewModel(initData) {
    var self = this;

    self.SearchKeyword = ko.observable();
    self.SelectedIndustry = ko.observable();
    self.SelectedIndustry.subscribe(function (newVal) {
        self.FilterProjects();
    });
    self.SelectedPractice = ko.observable();
    self.SelectedPractice.subscribe(function (newVal) {
        self.FilterProjects();
    });

    self.FilterProjects = function() {
        var searchProjectsUrl = $('#SearchProjectsUrl').val();
        $.ajax({
            type: 'POST',
            url: searchProjectsUrl,
            data: {
                practiceId: self.SelectedPractice(),
                industryId: self.SelectedIndustry(),
                keyword: self.SearchKeyword()
            },
            success: function (res) {
                if (res.status === "SPCD: OK") {
                    
                    self.DisplayedProjects.removeAll();

                    var DisplayedProjectsArray = jQuery.map(res.projects, function (val, i) {
                        self.DisplayedProjects.push(new ProjectModel(val, self));
                    });
                } else {
                    alert("There was an error updating the aboutMenu - " + res.status);
                }
            }
        });
    };

    self.AllIndustries = ko.observableArray([]);

    var IndustriesArray = jQuery.map(initData.Industries, function (val, i) {
        self.AllIndustries.push(new IndustryModel(val, self));
    });
    
    self.AllPracticies = ko.observableArray([]);

    var PracticiesArray = jQuery.map(initData.Practicies, function (val, i) {
        self.AllPracticies.push(new PracticeModel(val, self));
    });
    
    self.DisplayedProjects = ko.observableArray([]);

    var DisplayedProjectsArray = jQuery.map(initData.DisplayedProjects, function (val, i) {
        self.DisplayedProjects.push(new ProjectModel(val, self));
    });
    
    
}

mp.ProjectsLogic = (function ($) {
    "use strict";

    var init = function() {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialProjectsDataModel = $.parseJSON($('#initial-projects-data').val());
        var vm = new ProjectsSearchViewModel(initialProjectsDataModel);

        ko.applyBindings(vm, document.getElementById("projects-view"));
    };

    return { init: init };
}($));

mp.Projects = (function ($) {
    "use strict";

    var ready = $(function () {
        mp.ProjectsLogic.init();
    });
}($));