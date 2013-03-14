$(function () {

    var UsersInitialized = false;
    var PublicationsViewInitialized = false;
    var NewsViewInitialized = false;
    var LawNewsViewInitialized = false;
    var AwardsViewInitialized = false;
    var BrochuresViewInitialized = false;
    var IndustriesViewInitialized = false;
    var PracticiesViewInitialized = false;
    var TeamMemberViewInitialized = false;
    var AboutMenuViewInitialized = false;
    
    var ProjectsViewInitialized = false;

    $("#control-area").tabs({
        select: function (event, ui) {
            switch(ui.index)
            {
                case 0:
                    if (!NewsViewInitialized) {
                        NewsView.Init();
                        NewsViewInitialized = true;
                    }
                    break;
                case 1:
                    if (!PublicationsViewInitialized) {
                        PublicationsView.Init();
                        PublicationsViewInitialized = true;
                    }
                    break;
                case 2:
                    if (!BrochuresViewInitialized) {
                        BrochuresView.Init();
                        BrochuresViewInitialized = true;
                    }
                    break;
                case 3:
                    if (!AwardsViewInitialized) {
                        AwardsView.Init();
                        AwardsViewInitialized = true;
                    }
                    break;
                case 4:
                    if(!LawNewsViewInitialized) {
                        LawNewsView.Init();
                        LawNewsViewInitialized = true;
                    }
                    break;
                case 5:
                    if (!ProjectsViewInitialized) {
                        ProjectsView.Init();
                        ProjectsViewInitialized = true;
                    }
                    break;
                case 6:
                    if (!IndustriesViewInitialized) {
                        IndustriesView.Init();
                        IndustriesViewInitialized = true;
                    }
                    break;
                case 7:
                    if (!PracticiesViewInitialized) {
                        PracticiesView.Init();
                        PracticiesViewInitialized = true;
                    }
                    break;
                case 8:
                    if (!UsersInitialized) {
                        UsersView.Init();
                        UsersInitialized = true;
                    }
                    break;
                case 9:
                    if (!AboutMenuViewInitialized) {
                        AboutMenuView.Init();
                        AboutMenuViewInitialized = true;
                    }
                    break;
                case 10:
                    if (!TeamMemberViewInitialized) {
                        TeamMemberView.Init();
                        TeamMemberViewInitialized = true;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }
    });
    
    if (!NewsViewInitialized) {
        NewsView.Init();
        NewsViewInitialized = true;
    }
})