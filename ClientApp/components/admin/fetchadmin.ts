import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import { SpecialDate } from '../../models/specialdate';
import { Role } from '../../models/role';
import { Skill } from '../../models/skill';
import { Site } from '../../models/site';

@Component
export default class FetchAbsenceComponent extends Vue {
	specialdates: SpecialDate[] = [];
	roles: Role[] = [];
	skills: Skill[] = [];
	sites: Site[] = [];
	loadingSpecialDate: boolean = false;
	loadingRole: boolean = false;
	loadingSkill: boolean = false;
	loadingSite: boolean = false;
	searchSpecialDate: string = "";
	searchRole: string = "";
	searchSkill: string = "";
	searchSite: string = "";
	failed: boolean = false;
	errorMessage: string = "";
	dialog: boolean = false;
	dialogMessage: string = "";
	selectedSwitch: number = 0;
	selectedSpecialDate: number = 0;
	selectedRole: number = 0;
	selectedSkill: number = 0;
	selectedSite: number = 0;

	headersSpecialDate: object[] = [
		{ text: 'Name', value: 'name' },
		{ text: 'Date', value: 'date' }
	];

	headersRole: object[] = [
		{ text: 'Name', value: 'name' }
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

	mounted() {
		this.loadSpecialDates();
		this.loadRoles();
		this.loadSkills();
		this.loadSites();
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

	createSpecialDate() {
		this.$router.push("/createspecialdate");
	}

	createRole() {
		this.$router.push("/createrole");
	}

	createSkill() {
		this.$router.push("/createskill");
	}

	createSite() {
		this.$router.push("/createsite");
	}

	editSpecialDate(id: number) {
		this.$router.push("/editspecialdate/" + id);
	}

	editRole(id: number) {
		this.$router.push("/editrole/" + id);
	}

	editSkill(id: number) {
		this.$router.push("/editskill/" + id);
	}

	editSite(id: number) {
		this.$router.push("/editsite/" + id);
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

	openSkillDelete(selected: number) {
		this.selectedSkill = selected;
		this.selectedSwitch = 2;
		this.dialogMessage = "Are you sure you want to delete this skill?";
		this.dialog = true;
	}

	openSiteDelete(selected: number) {
		this.selectedSite = selected;
		this.selectedSwitch = 3;
		this.dialogMessage = "Are you sure you want to delete this site?";
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
				this.deleteSkill();
				break;
			case 2:
				this.deleteSite();
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
}
