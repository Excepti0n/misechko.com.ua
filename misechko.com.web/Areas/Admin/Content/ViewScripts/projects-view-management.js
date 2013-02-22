function ProjectModel(pubData, parent) {
    var self = this;

    self.Id = pubData.Id;
    self.ProjectName = pubData.Headline;
    self.ProjectPath = pubData.LinkPath;
    self.ProjectHREF = '/Read' + self.ProjectPath;
    self.DateCreated = pubData.PublishDate;
    self.Type = pubData.Type;

    
    self.Remove = function () {
        var removePubUrl = $('#RemoveProjectUrl').val();
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
                    alert("There was an error removing the project - " + res);
                }
            }
        });
    };
    
    self.UpdateProject = function () {
        var updatePubUrl = $('#UpdateProjectUrl').val();
        $.ajax({
            type: 'POST',
            url: updatePubUrl,
            data: {
                id: self.Id,
                projectName: self.ProjectName,
                dateCreated: self.DateCreated
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemovePub(self);
                } else {
                    alert("There was an error removing the project - " + res);
                }
            }
        });
    };
}

function ProjectsManagementViewModel(initData) {
    var self = this;

    self.RemovePub = function (pubVM) {
        self.Projects.remove(pubVM);
    };

    self.NewProjectName = ko.observable('').extend({ required: true });
    
    self.Projects = ko.observableArray([]);

    var projectsArray = jQuery.map(initData.Projects, function (val, i) {
        self.Projects.push(new ProjectModel(val, self, i == 0));
    });

    self.AddNewProject = function () {
        var addProjectUrl = $('#AddProjectUrl').val();
        $.ajax({
            type: 'POST',
            url: addProjectUrl,
            data: {
                projectName: self.NewProjectName()
            },
            success: function (res) {
                if (res.status === "SPCD: PJADDED") {
                    var pbVM = new ProjectModel(res.project, self, false);
                    self.Projects.push(pbVM);
                    self.NewProjectName('');
                } else {
                    alert("There was an error adding the project: " + res.status);
                }
            }
        });
    };
}

var ProjectsView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialProjectsDataObject = $.parseJSON($('#initial-projects-list').val());
        var vm = new ProjectsManagementViewModel(initialProjectsDataObject);

        ko.applyBindings(vm, document.getElementById("projects-management-view"));
    }
};