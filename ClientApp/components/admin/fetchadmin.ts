import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { SpecialDate } from '../../models/specialdate';
import { Skill } from '../../models/skill';
import { Site } from '../../models/site';

@Component
export default class FetchAbsenceComponent extends Vue {
	specialdates: SpecialDate[] = [];
	skills: Skill[] = [];
	sites: Site[] = [];
	loadingSpecialDate: boolean = false;
	loadingSkill: boolean = false;
	loadingSite: boolean = false;
	searchSpecialDate: string = "";
	searchSkill: string = "";
	searchSite: string = "";

	headersSpecialDate: object[] = [
		{ text: 'Name', value: 'name' },
		{ text: 'Date', value: 'date' }
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

	createSkill() {
		this.$router.push("/createskill");
	}

	createSite() {
		this.$router.push("/createsite");
	}

	editSpecialDate(id: number) {
		this.$router.push("/editspecialdate/" + id);
	}

	editSkill(id: number) {
		this.$router.push("/editskill/" + id);
	}

	editSite(id: number) {
		this.$router.push("/editsite/" + id);
	}

	deleteSpecialDate(id: number) {
		var ans = confirm("Do you want to delete this Special Date?");
		if (ans) {
			fetch('api/Admin/DeleteSpecialDate?id=' + id, {
				method: 'DELETE'
			})
				.then(response => response.json() as Promise<number>)
				.then(data => {
					if (data < 1) {
						alert("Failed to delete Special Date. Please make sure you are still connected?");
					} else {
						this.loadSpecialDates();
					}
				})
		}
	}

	deleteSkill(id: number) {
		var ans = confirm("Do you want to delete this Skill?");
		if (ans) {
			fetch('api/Admin/DeleteSkill?id=' + id, {
				method: 'DELETE'
			})
				.then(response => response.json() as Promise<number>)
				.then(data => {
					if (data < 1) {
						alert("Failed to delete Skill. Please make sure you are still connected?");
					} else {
						this.loadSkills();
					}
				})
		}
	}

	deleteSite(id: number) {
		var ans = confirm("Do you want to delete this Site?");
		if (ans) {
			fetch('api/Admin/DeleteSite?id=' + id, {
				method: 'DELETE'
			})
				.then(response => response.json() as Promise<number>)
				.then(data => {
					if (data < 1) {
						alert("Failed to delete Site. Please make sure you are still connected?");
					} else {
						this.loadSites();
					}
				})
		}
	}
}
