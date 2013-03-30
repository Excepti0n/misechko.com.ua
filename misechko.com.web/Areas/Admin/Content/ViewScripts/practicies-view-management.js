function PracticeModel(practiceData, parent) {
    var self = this;

    self.Id = practiceData.Id;
    self.PracticeName = practiceData.Headline;
    self.PracticePath = practiceData.LinkPath;
    self.PracticeHREF = '/Read' + self.PracticePath;
    self.DateCreated = practiceData.PublishDate;
    self.Type = practiceData.Type;

    self.IndexChanged = ko.observable(false);

    self.ItemIndex = ko.observable(practiceData.Index);
    self.ItemIndex.subscribe(function () {
        self.IndexChanged(true);
    });

    self.PracticeProjectsChanged = ko.observable(false);
    self.PracticePublicationsChanged = ko.observable(false);

    self.PracticeProjects = ko.observableArray(practiceData.Projects);
    self.PracticeProjects.subscribe(function () {
        self.PracticeProjectsChanged(true);
    });
    self.PracticePublications = ko.observableArray(practiceData.Publications);
    self.PracticePublications.subscribe(function () {
        self.PracticePublicationsChanged(true);
    });

    self.ParentProjects = ko.computed(function () {
        return jQuery.map(parent.AllProjects(), function (val, i) {
            return val;
        });
    });
    
    self.ParentPublications = ko.computed(function () {
        return jQuery.map(parent.AllPublications(), function (val, i) {
            return val;
        });
    });
    
    self.Remove = function () {
        var removePubUrl = $('#RemovePracticeUrl').val();
        $.ajax({
            type: 'POST',
            url: removePubUrl,
            data: {
                id: self.Id
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemovePractice(self);
                } else {
                    alert("There was an error removing the Practice - " + res);
                }
            }
        });
    };
    
    self.UpdatePractice = function () {
        var updatePubUrl = $('#UpdatePracticeUrl').val();
        $.ajax({
            type: 'POST',
            url: updatePubUrl,
            data: {
                id: self.Id,
                PracticeName: self.PracticeName,
                projectsInPractice: JSON.stringify(self.PracticeProjects()),
                publicationsInPractice: JSON.stringify(self.PracticePublications()),
                dateCreated: self.DateCreated,
                index: self.ItemIndex()
            },
            success: function (res) {
                if (res.status === "SPCD: OK") {
                    self.PracticePublicationsChanged(false);
                    self.PracticeProjectsChanged(false);
                    self.IndexChanged(false);
                } else {
                    alert("There was an error updating the Practice - " + res.status);
                }
            }
        });
    };
}

function PracticeProjectModel(modelData, parent) {
    var self = this;

    self.ProjectName = modelData.Headline;
}

function PracticePublicationModel(modelData, parent) {
    var self = this;

    self.PublicationName = modelData.Headline;
}

function PracticiesManagementViewModel(initData) {
    var self = this;

    self.RemovePractice = function (practiceVM) {
        self.Practicies.remove(practiceVM);
    };

    self.NewPracticeName = ko.observable('').extend({ required: true });
    
    self.AllProjects = ko.observableArray([]);

    var ProjectsArray = jQuery.map(initData.AllProjects, function (val, i) {
        self.AllProjects.push(new PracticeProjectModel(val, self));
    });

    self.AllPublications = ko.observableArray([]);

    var PublicationsArray = jQuery.map(initData.AllPublications, function (val, i) {
        self.AllPublications.push(new PracticePublicationModel(val, self));
    });
    
    self.Practicies = ko.observableArray([]);

    var PracticiesArray = jQuery.map(initData.Practicies, function (val, i) {
        self.Practicies.push(new PracticeModel(val, self, i == 0));
    });
    
    self.AddNewPractice = function () {
        var addPracticeUrl = $('#AddPracticeUrl').val();
        $.ajax({
            type: 'POST',
            url: addPracticeUrl,
            data: {
                PracticeName: self.NewPracticeName()
            },
            success: function (res) {
                if (res.status === "SPCD: PRADDED") {
                    var pbVM = new PracticeModel(res.practice, self, false);
                    self.Practicies.push(pbVM);
                    self.NewPracticeName('');
                } else {
                    alert("There was an error adding the Practice: " + res.status);
                }
            }
        });
    };
}

var PracticiesView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialPracticiesDataObject = $.parseJSON($('#initial-practicies-list').val());
        var vm = new PracticiesManagementViewModel(initialPracticiesDataObject);

        ko.applyBindings(vm, document.getElementById("practicies-management-view"));
    }
};