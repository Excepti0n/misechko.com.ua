function UserModel(userData, parent) {
    var self = this;

    self.UserName = userData.UserName;
    self.DisplayName = ko.observable(userData.DisplayName);
    self.DisplayNameChanged = ko.observable(false);

    self.UserRolesChanged = ko.observable(false);

    self.Roles = ko.observableArray(userData.Roles);
    self.Roles.subscribe(function() {
        self.UserRolesChanged(true);
    });

    self.ParentRoles = ko.computed(function() {
        return jQuery.map(parent.Roles(), function (val, i) {
            return val.RoleName;
        }); 
    });

    self.SaveNewRoles = function() {
        var addUserToRolesUrl = $('#AddUserToRolesUrl').val();
        $.ajax({
            type: 'POST',
            url: addUserToRolesUrl,
            data: {
                userName: self.UserName,
                newRoles: JSON.stringify(self.Roles())
            },
            dataType: "json",
            success: function (res) {
                if (res === "SPCD: OK") {
                    self.UserRolesChanged(false);
                } else {
                    alert("There was an error adding user to roles - " + res);
                }
            }
        });
    };

    self.DisplayName.subscribe(function() {
        self.DisplayNameChanged(true);
    });

    self.NewPasswordChanged = ko.observable(false);
    
    self.NewPassword = ko.observable('');
    self.NewPassword.subscribe(function() {
        self.NewPasswordChanged(true);
    });

    self.UpdatingPassword = ko.observable(false);
    self.EnableUpdatePassword = function() {
        self.UpdatingPassword(true);
    };
    
    self.Remove = function () {
        var removeUserUrl = $('#DeleteUserUrl').val();
        $.ajax({
            type: 'POST',
            url: removeUserUrl,
            data: {
                userName: self.UserName
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    parent.RemoveUser(self);
                } else {
                    alert("There was an error removing the user - " + res);
                }
            }
        });
    };

    self.SaveUpdatedPassword = function() {
        var updatePassUrl = $('#UpdatePasswordUrl').val();
        $.ajax({
            type: 'POST',
            url: updatePassUrl,
            data: {
                userName: self.UserName,
                newPass: self.NewPassword()
            },
            success: function (res) {
                if (res === "SPCD: OK") {
                    self.UpdatingPassword(false);
                    self.NewPassword('');
                    self.NewPasswordChanged(false);
                } else {
                    alert("There was an error updating password");
                }
            }
        });
    };

    self.FacebookId = userData.FacebookId;

    self.SaveDisplayName = function() {
        var updateNameUrl = $('#UpdateDisplayNameUrl').val();
        $.ajax({
            type: 'POST',
            url: updateNameUrl,
            data: {
                userName: self.UserName,
                newDisplayName: self.DisplayName()
            },
            success: function(res) {
                if (res === "SPCD: USRNMUPDATED") {
                    self.DisplayNameChanged(false);
                } else {
                    alert("There was an error updating the display name: " + res);
                }
            }
        });
    };
}

function RoleModel(roleData, parent, selected) {
    var self = this;

    self.RoleName = roleData.RoleName;
    self.UsersCount = ko.observable(roleData.RoleUsersCount);
    self.Selected = ko.observable(selected);

    self.LoadUsersInRole = function() {
        var getUsersInRoleUrl = $('#GetUsersInRoleUrl').val();
        $.ajax({
            type: 'GET',
            url: getUsersInRoleUrl,
            data: {
                roleName: self.RoleName
            },
            success: function (res) {
                if (res.status === "SPCD: OK") {
                    parent.UsersInRole.removeAll();
                    var userModelsArray = jQuery.map(res.users, function (val, i) {
                        parent.UsersInRole.push(new UserModel(val, parent));
                    });
                } else {
                    alert("There was an error retrieveing users in role: " + res.status);
                }
            }
        });
    };

    self.Select = function () {
        if (!self.Selected()) {
            parent.DeselectOtherRoles(self.RoleName);
            self.Selected(true);
            self.LoadUsersInRole();
        }
        return true;
    };

    self.HasAnAdminFeatures = ko.observable(roleData.AdminFeaturesAvailable);

    self.HasAnAdminFeatures.subscribe(function(newValue) {
        var setRoleAdminFeaturesAvailabilityUrl = $('#SetRoleAdminFeaturesAvailabilityUrl').val();
        $.ajax({
            type: 'POST',
            url: setRoleAdminFeaturesAvailabilityUrl,
            data: {
                roleName: self.RoleName,
                areAdminFeaturesAvailable: newValue
            },
            success: function (res) {
                if (res === "SPCD: AFSET") {
                } else {
                    alert("There was an error updating the admin features availability value: " + res);
                }
            }
        });
    });

    self.Remove = function() {
        var removeRoleUrl = $('#RemoveRoleUrl').val();
        $.ajax({
            type: 'POST',
            url: removeRoleUrl,
            data: {
                roleName: self.RoleName
            },
            success: function (res) {
                if (res === "SPCD: RLREMOVED") {
                    parent.RemoveRole(self);
                } else {
                    alert("There was an error removing the role: " + res);
                }
            }
        });
    };
}

function PermissionModel(permissionData, parent, selected) {
    var self = this;

    self.Id = permissionData.Id;

    self.PermissionName = permissionData.PermissionName;
    
    self.PermissionRolesChanged = ko.observable(false);

    self.Roles = ko.observableArray(permissionData.Roles);
    self.Roles.subscribe(function () {
        self.PermissionRolesChanged(true);
    });
    
    self.CAsChanged = ko.observable(false);

    self.CAs = ko.observable([permissionData.ControllerAndAction]);
    self.CAs.subscribe(function () {
        self.CAsChanged(true);
    });
    
    self.ParentRoles = ko.computed(function () {
        return jQuery.map(parent.Roles(), function (val, i) {
            return val.RoleName;
        });
    });
    
    self.AllCAs = ko.computed(function () {
        return parent.ControllersAndActions();
    });
    
    self.Remove = function () {
        var removePermissionUrl = $('#RemovePermissionUrl').val();
        $.ajax({
            type: 'POST',
            url: removePermissionUrl,
            data: {
                permissionId: self.Id
            },
            success: function (res) {
                if (res === "SPCD: PRREMOVED") {
                    parent.RemovePermission(self);
                } else {
                    alert("There was an error removing the permission: " + res);
                }
            }
        });
    };
    
    self.SaveNewPermissionRoles = function () {
        var addRoleToPermissionUrl = $('#AddRolesToPermissionUrl').val();
        $.ajax({
            type: 'POST',
            url: addRoleToPermissionUrl,
            data: {
                permissionId: self.Id,
                newRoles: JSON.stringify(self.Roles())
            },
            dataType: "json",
            success: function (res) {
                if (res === "SPCD: OK") {
                    self.PermissionRolesChanged(false);
                } else {
                    alert("There was an error adding roles to permission - " + res);
                }
            }
        });
    };
    
    self.SaveNewPermissionCA = function () {
        var addCAToPermissionUrl = $('#AddCAToPermissionUrl').val();
        $.ajax({
            type: 'POST',
            url: addCAToPermissionUrl,
            data: {
                permissionId: self.Id,
                newRoles: JSON.stringify(self.CAs())
            },
            dataType: "json",
            success: function (res) {
                if (res === "SPCD: OK") {
                    self.CAsRolesChanged(false);
                } else {
                    alert("There was an error setting CAs to permission - " + res);
                }
            }
        });
    };
}

function UsersManagementViewModel(initData) {
    var self = this;

    self.NewRoleName = ko.observable('');
    self.NewPermissionName = ko.observable('');
    self.NewPermissionCAs = ko.observableArray([]);

    self.RemovePermission = function (permVM) {
        self.Permissions.remove(permVM);
    };

    self.RemoveRole = function(roleVM) {
        self.Roles.remove(roleVM);
    };
    
    self.RemoveUser = function (userVM) {
        self.UsersInRole.remove(userVM);
    };
    
    self.isAddUserDialogVisible = ko.observable(false);

    self.NewUserName = ko.observable('').extend({ required: true });
    self.NewUserPassword = ko.observable('').extend({ required: true });
    self.NewUserDisplayName = ko.observable('');
    self.NewUserRoles = ko.observable('').extend({ required: true });;
    self.NewUserEmail = ko.observable('');

    self.SelectedRoleName = ko.observable('');
    
    self.NewUserErrors = ko.validation.group(self);

    self.addUserDialogOptions = {
        autoOpen: false,
        modal: true,
        height: 600,
        width: 800,
        title: 'Добавление нового пользователя',
        open: function () {
            $('.chzn-select').chosen();
        },
        buttons: {
            'Добавить пользователя': function (e) {
                if (!self.NewUserErrors().length == 0) {
                    self.errors.showAllMessages();
                    return;
                }
                var addUserUrl = $('#AddNewUserUrl').val();
                $.ajax({
                    type: 'POST',
                    url: addUserUrl,
                    data: {
                        userName: self.NewUserName(),
                        pass: self.NewUserPassword(),
                        displayName: self.NewUserDisplayName(),
                        email: self.NewUserEmail(),
                        roles: JSON.stringify(self.NewUserRoles())
                    },
                    dataType: "json",
                    success: function (res) {
                        if (res.status === "SPCD: OK") {

                            var index = $.inArray(self.SelectedRoleName(), res.user.Roles);
                            
                            if(index != -1) {
                                self.UsersInRole.push(new UserModel(res.user, self));
                            }

                            self.isAddUserDialogVisible(false);
                            self.NewUserName('');
                            self.NewUserPassword('');
                            self.NewUserDisplayName('');
                            self.NewUserRoles('');
                            self.NewUserEmail('');
                        } else {
                            alert("There was an error adding the user: " + res.status);
                        }
                    }
                });
            },
            'Отмена': function () { self.isAddUserDialogVisible(false); }
        }
    };

    self.ShowAddUserDialog = function() {
        self.isAddUserDialogVisible(true);
    };

    self.UsersInRole = ko.observableArray([]);
    self.Roles = ko.observableArray([]);
    
    var roleModelsArray = jQuery.map(initData.RoleModels, function (val, i) {
        self.Roles.push(new RoleModel(val, self, i == 0));
        if(i == 0) {
            self.SelectedRoleName(val.RoleName);
        }
    });

    var userModelsArray = jQuery.map(initData.UsersInFirstRole, function (val, i) {
        self.UsersInRole.push(new UserModel(val, self));
    });

    self.AddNewRole = function() {
        var addRoleUrl = $('#AddRoleUrl').val();
        $.ajax({
            type: 'POST',
            url: addRoleUrl,
            data: {
                roleName: self.NewRoleName()
            },
            success: function (res) {
                if (res === "SPCD: RLADDED") {
                    var rlVM = new RoleModel({ RoleName: self.NewRoleName(), RoleUsersCount: 0 }, self, false);
                    self.Roles.push(rlVM);
                    self.NewRoleName('');
                } else {
                    alert("There was an error adding the role: " + res);
                }
            }
        });
    };

    self.DeselectOtherRoles = function(newSelectedRole) {
        self.SelectedRoleName(newSelectedRole);
        ko.utils.arrayForEach(self.Roles(), function (roleModel) {
            roleModel.Selected(false);
        });
    };
    
    //Permissions logic code

    self.Permissions = ko.observableArray([]);
    
    self.ControllersAndActions = ko.observableArray(initData.AllControllersAndActions);

    var permissionsModelsArray = jQuery.map(initData.PermissionsModels, function (val, i) {
        self.Permissions.push(new PermissionModel(val, self, i == 0));
    });
    
    self.AddNewPermission = function () {
        var AddNewPermissionUrl = $('#AddNewPermissionUrl').val();
        $.ajax({
            type: 'POST',
            url: AddNewPermissionUrl,
            data: {
                permissionName: self.NewPermissionName(),
                CAs: JSON.stringify(self.NewPermissionCAs())
            },
            success: function (res) {
                if (res.status === "SPCD: PRADDED") {
                    var prVM = new PermissionModel(res.addedModel, self, false);
                    self.Permissions.push(prVM);
                    self.NewPermissionName('');
                    self.NewPermissionCAs(['']);
                } else {
                    alert("There was an error adding the permission: " + res.status);
                }
            }
        });
    };
}

var UsersView = {
    Init: function () {
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            messageTemplate: null
        });

        var initialRolesAndUsersDataObject = $.parseJSON($('#initial-users-and-roles-list').val());
        var vm = new UsersManagementViewModel(initialRolesAndUsersDataObject);
       
        ko.applyBindings(vm, document.getElementById("users-management-view"));
    }
};