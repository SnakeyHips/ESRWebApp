import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import { SpecialDate } from '../../models/specialdate';
import { Role } from '../../models/role';
import { Skill } from '../../models/skill';
import { Site } from '../../models/site';
import { Template } from '../../models/template';
import { AbsenceType } from '../../models/absencetype';

@Component
export default class FetchAbsenceComponent extends Vue {
	specialdates: SpecialDate[] = [];
	roles: Role[] = [];
	templates: Template[] = [];
	skills: Skill[] = [];
	sites: Site[] = [];
	absencetypes: AbsenceType[] = [];
	loadingSpecialDate: boolean = false;
	loadingRole: boolean = false;
	loadingTemplate: boolean = false;
	loadingSkill: boolean = false;
	loadingSite: boolean = false;
	loadingAbsenceType: boolean = false;
	searchSpecialDate: string = "";
	searchRole: string = "";
	searchTemplate: string = "";
	searchSkill: string = "";
	searchSite: string = "";
	searchAbsenceType: string = "";
	failed: boolean = false;
	errorMessage: string = "";
	dialog: boolean = false;
	dialogMessage: string = "";
	selectedSwitch: number = 0;
	selectedSpecialDate: number = 0;
	selectedRole: number = 0;
	selectedTemplate: number = 0;
	selectedSkill: number = 0;
	selectedSite: number = 0;
	selectedAbsenceType: number = 0;

	headersSpecialDate: object[] = [
		{ text: 'Name', value: 'name' },
		{ text: 'Date', value: 'date' }
	];

	headersRole: object[] = [
		{ text: 'Name', value: 'name' },
		{ text: 'Count', value: 'count' }
	];

	headersTemplate: object[] = [
		{ text: 'Name', value: 'name' },
		{ text: 'Roles', value: 'roles' }
	];

	headersSkill: object[] = [
		{ text: 'Role', value: 'role' },
		{ text: 'Name', value: 'name' }
	];

	headersSite: object[] = [
		{ text: 'Name', value: 'name' },
		{ text: 'Type', value: 'type' },
		{ text: 'Times', value: 'times' }
	];

	headersAbsenceType: object[] = [
		{ text: 'Name', value: 'name' },
		{ text: 'Colour', value: 'colour' },
	];

	mounted() {
		this.loadSpecialDates();
		this.loadRoles();
		this.loadTemplates();
		this.loadSkills();
		this.loadSites();
		this.loadAbsenceTypes();
	}

	loadSpecialDates() {
		this.loadingSpecialDate = true;
		fetch('api/Admin/GetSpecialDates')
			.then(response => response.json() as Promise<SpecialDate[]>)
			.then(data => {
				this.specialdates = data;
				this.loadingSpecialDate = false;
			});
	}

	loadRoles() {
		this.loadingRole = true;
		fetch('api/Admin/GetRoles')
			.then(response => response.json() as Promise<Role[]>)
			.then(data => {
				this.roles = data;
				this.loadingRole = false;
			});
	}

	loadTemplates() {
		this.loadingTemplate = true;
		fetch('api/Admin/GetTemplates')
			.then(response => response.json() as Promise<Template[]>)
			.then(data => {
				this.templates = data;
				this.loadingTemplate = false;
			});
	}

	loadSkills() {
		this.loadingSkill = true;
		fetch('api/Admin/GetSkills')
			.then(response => response.json() as Promise<Skill[]>)
			.then(data => {
				this.skills = data;
				this.loadingSkill= false;
			});
	}

	loadSites() {
		this.loadingSite = true;
		fetch('api/Admin/GetSites')
			.then(response => response.json() as Promise<Site[]>)
			.then(data => {
				this.sites = data;
				this.loadingSite = false;
			});
	}

	loadAbsenceTypes() {
		this.loadingAbsenceType = true;
		fetch('api/Admin/GetAbsenceTypes')
			.then(response => response.json() as Promise<AbsenceType[]>)
			.then(data => {
				this.absencetypes = data;
				this.loadingAbsenceType = false;
			});
	}

	createSpecialDate() {
		this.$router.push("/createspecialdate");
	}

	createRole() {
		this.$router.push("/createrole");
	}

	createTemplate() {
		this.$router.push("/createtemplate");
	}

	createSkill() {
		this.$router.push("/createskill");
	}

	createSite() {
		this.$router.push("/createsite");
	}

	createAbsenceType() {
		this.$router.push("/createabsencetype");
	}

	editSpecialDate(id: number) {
		this.$router.push("/editspecialdate/" + id);
	}

	editRole(id: number) {
		this.$router.push("/editrole/" + id);
	}

	editTemplate(id: number) {
		this.$router.push("/edittemplate/" + id);
	}

	editSkill(id: number) {
		this.$router.push("/editskill/" + id);
	}

	editSite(id: number) {
		this.$router.push("/editsite/" + id);
	}

	editAbsenceType(id: number) {
		this.$router.push("/editabsencetype/" + id);
	}

	openSpecialDateDelete(selected: number) {
		this.selectedSpecialDate = selected;
		this.selectedSwitch = 0;
		this.dialogMessage = "Are you sure you want to delete this special date?";
		this.dialog = true;
	}

	openRoleDelete(selected: number) {
		this.selectedRole = selected;
		this.selectedSwitch = 1;
		this.dialogMessage = "Are you sure you want to delete this role?";
		this.dialog = true;
	}

	openTemplateDelete(selected: number) {
		this.selectedTemplate = selected;
		this.selectedSwitch = 2;
		this.dialogMessage = "Are you sure you want to delete this template?";
		this.dialog = true;
	}

	openSkillDelete(selected: number) {
		this.selectedSkill = selected;
		this.selectedSwitch = 3;
		this.dialogMessage = "Are you sure you want to delete this skill?";
		this.dialog = true;
	}

	openSiteDelete(selected: number) {
		this.selectedSite = selected;
		this.selectedSwitch = 4;
		this.dialogMessage = "Are you sure you want to delete this site?";
		this.dialog = true;
	}

	openAbsenceTypeDelete(selected: number) {
		this.selectedAbsenceType = selected;
		this.selectedSwitch = 5;
		this.dialogMessage = "Are you sure you want to delete this absence type?";
		this.dialog = true;
	}

	deleteSwitch() {
		switch (this.selectedSwitch) {
			case 0:
				this.deleteSpecialDate();
				break;
			case 1:
				this.deleteRole();
				break;
			case 2:
				this.deleteTemplate();
				break;
			case 3:
				this.deleteSkill();
				break;
			case 4:
				this.deleteSite();
				break;
			case 5:
				this.deleteAbsenceType();
				break;
		}
	}

	deleteSpecialDate() {
		this.failed = false;
		this.dialog = false;
		fetch('api/Admin/DeleteSpecialDate?id=' + this.selectedSpecialDate, {
			method: 'DELETE'
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					this.errorMessage = "Failed to delete special date!";
					this.failed = true;
				} else {
					this.loadSpecialDates();
				}
			})
	}

	deleteRole() {
		this.failed = false;
		this.dialog = false;
		fetch('api/Admin/DeleteRole?id=' + this.selectedRole, {
			method: 'DELETE'
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					this.errorMessage = "Failed to delete role!";
					this.failed = true;
				} else {
					this.loadRoles();
				}
			})
	}

	deleteTemplate() {
		this.failed = false;
		this.dialog = false;
		fetch('api/Admin/DeleteTemplate?id=' + this.selectedTemplate, {
			method: 'DELETE'
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					this.errorMessage = "Failed to delete template!";
					this.failed = true;
				} else {
					this.loadTemplates();
				}
			})
	}

	deleteSkill() {
		this.failed = false;
		this.dialog = false;
		fetch('api/Admin/DeleteSkill?id=' + this.selectedSkill, {
			method: 'DELETE'
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					this.errorMessage = "Failed to delete skill!";
					this.failed = true;
				} else {
					this.loadSkills();
				}
			})
	}

	deleteSite() {
		this.failed = false;
		this.dialog = false;
		fetch('api/Admin/DeleteSite?id=' + this.selectedSite, {
			method: 'DELETE'
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					this.errorMessage = "Failed to delete site!";
					this.failed = true;
				} else {
					this.loadSites();
				}
			})
	}

	deleteAbsenceType() {
		this.failed = false;
		this.dialog = false;
		fetch('api/Admin/DeleteAbsenceType?id=' + this.selectedAbsenceType, {
			method: 'DELETE'
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					this.errorMessage = "Failed to delete absence type!";
					this.failed = true;
				} else {
					this.loadAbsenceTypes();
				}
			})
	}
}
