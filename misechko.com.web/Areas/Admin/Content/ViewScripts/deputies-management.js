/// <reference path="~/Scripts/knockout-2.1.0.debug.js" />

function guidGenerator() {
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

function DeputyManagementViewModel(deputy) {
    var self = this;

    self.HasUnsavedChanges = ko.observable(deputy.NewBorn);

    self.Id = deputy.Id;

    self.FirstName = ko.observable(deputy.FirstName).extend({
        required: true
    });
    
    self.FirstName.subscribe(function () {
        self.HasUnsavedChanges(true);
    });
    
    self.LastName = ko.observable(deputy.LastName).extend({
        required: true
    });
    
    self.LastName.subscribe(function () {
        self.HasUnsavedChanges(true);
    });
    
    self.Patronimic = ko.observable(deputy.Patronimic);
    
    self.Patronimic.subscribe(function () {
        self.HasUnsavedChanges(true);
    });
    
    self.Bio = ko.observable(deputy.Bio);
    
    self.Bio.subscribe(function () {
        self.HasUnsavedChanges(true);
    });

    self.Gender = ko.observable(deputy.Gender);
    
    self.Gender.subscribe(function () {
        self.HasUnsavedChanges(true);
    });

    self.HasImage = ko.observable(deputy.HasImage);

    self.D = new Date();

    self.CurTime = ko.observable(self.D.getTime());
    
    self.ImgSrc = ko.computed(function () {
        return '/Content/party-view/images/deputies/' + self.Id + '.png?' + self.CurTime();
    });

    self.DisplayName = ko.computed(function () {
        var res = "";

        if (self.Patronimic()) {
            res = self.FirstName() + " " + self.Patronimic() + " " + self.LastName();
        } else res = self.FirstName() + " " + self.LastName();

        return res;
    });

    self.AddImageContent = ko.observable('Добавить фото депутата');
    self.IsInUploadMode = ko.observable(false);

    self.ChangeImage = function() {
        self.HasImage(false);
        self.AddImage();
    };

    self.AddImage = function () {
        if (!self.IsInUploadMode()) {

            self.IsInUploadMode(true);
            //Initializing inline image adding button
            var $uploadControlForm = $('<div><form class="fileupload" id="fileupload-deputy-photo" method="POST" enctype="multipart/form-data"> \
			<div class="row fileupload-buttonbar"> \
				<div class="span7"> \
					<!-- The fileinput-button span is used to style the file input field as button --> \
					<span class="btn btn-success fileinput-button"> \
						<i class="icon-plus icon-white"></i> \
						<span>Выбрать</span> \
						<input type="file" name="files[]"> \
					</span> \
				</div> \
				<div class="span5"> \
					<!-- The global progress bar --> \
					<div class="progress progress-success progress-striped active fade"> \
						<div class="bar" style="width:0%;"></div> \
					</div> \
				</div> \
			</div> \
			<!-- The loading indicator is shown during image processing --> \
			<div class="fileupload-loading"></div> \
			<!-- The table listing the files available for upload/download --> \
			<table class="table table-striped"><tbody class="files"></tbody></table> \
		    </form></div>');

            $uploadControlForm.find('.fileupload').attr("action", $("#FileHandlerUrl").val());

            self.AddImageContent($uploadControlForm.html());

            var $form = $('#deputies-management-view').find('.fileupload');

            $form.fileupload();

            $form.fileupload('option', {
                formData: { what: 'dep-photo', objectId: self.Id },
                maxFileSize: 50000000,
                resizeMaxWidth: 1920,
                resizeMaxHeight: 1200,
                uploadTemplateId: 'template-upload-empty',
                downloadTemplateId: 'template-download-empty'
            });

            $form.bind('fileuploaddone', function (e, data) {
                self.AddImageContent('Начинаю замену...');
                self.DisableChangeImage();
                self.HasImage(true);
                self.D = new Date();
                self.CurTime(self.D.getTime());
                self.IsInUploadMode(false);
            });
            
        } else {
            return true;
        }
    };

    self.ChangeBtnVisible = ko.observable(false);
    self.EnableChangeImage = function() {
        self.ChangeBtnVisible(true);
    };

    self.DisableChangeImage = function() {
        self.ChangeBtnVisible(false);
    };

    self.SaveChangesText = ko.observable('Сохранить изменения');
    self.IsSavingDeputyInfo = ko.observable(false);

    self.UpdateDeputy = function () {
        if (!self.IsSavingDeputyInfo()) {
            
            if (!ko.validation.validateObservable(self.FirstName)) {
                alert("First name is not valid!");
                return;
            }
            
            if (!ko.validation.validateObservable(self.LastName)) {
                alert("Last name is not valid!");
                return;
            }

            self.IsSavingDeputyInfo(true);
            $.ajax({
                url: $('#UpdateDeputyInfoUrl').val(),
                global: false,
                type: "POST",
                data: {
                    id: deputy.Id,
                    partyId: deputy.PartyId,
                    firstName: self.FirstName(),
                    lastName: self.LastName(),
                    patronimic: self.Patronimic(),
                    bio: self.Bio(),
                    gender: self.Gender(),
                    create: deputy.NewBorn
                },
                beforeSend: function () {
                    self.SaveChangesText('Сохраняю...');
                }

            }).done(function (resCode) {
                self.HasUnsavedChanges(false);
                self.SaveChangesText('Сохранить изменения');
                self.IsSavingDeputyInfo = ko.observable(false);
            });
        }
    };
}


function DeputiesManagementViewModel(initData) {
    var self = this;

    var deputyModelsArray = jQuery.map(initData.Deputies, function (val, i) {
        var depData = $.extend({}, val, { NewBorn: false, PartyId: initData.PartyId});
        return (new DeputyManagementViewModel(depData));
    });
    
    self.Deputies = ko.observableArray(deputyModelsArray);

    self.AddDeputy = function () {
        var id = guidGenerator();
        var deputyData = { Id: id, PartyId: initData.PartyId, FirstName: "", LastName: "", Patronimic: "", HasImage: false, Bio: "", NewBorn: true };
        self.Deputies.unshift(new DeputyManagementViewModel(deputyData));
    };

}

var DeputiesManagement = {
    Init: function (vmData) {
        var vm = new DeputiesManagementViewModel(vmData);
        ko.applyBindings(vm, document.getElementById("deputies-management-view"));
    }
}