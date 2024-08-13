import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { preventUnsavedChagesGuard } from './_guards/prevent-unsaved-chages.guard';
import { memberDetailedResolver } from './_resolvers/member-detailed.resolver';
import { AdminPanelComponent } from './adming/admin-panel/admin-panel.component';
import { adminGuard } from './_guards/admin.guard';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [
            { path: 'members', component: MemberListComponent, canActivate: [authGuard] },
            { path: 'members/:username', component: MemberDetailComponent ,
                 resolve: {member:memberDetailedResolver}},
            { path: 'member/edit', 
                component: MemberEditComponent ,
                 canDeactivate:[preventUnsavedChagesGuard]},
            { path: 'lists', component: ListsComponent },
            { path: 'messages', component: MessagesComponent },
            {path : 'admin' , component: AdminPanelComponent, canActivate: [adminGuard] }
        ]
    },
    {path: 'errors', component: TestErrorsComponent},
    { path: '**', component: HomeComponent, pathMatch: 'full' }
];
