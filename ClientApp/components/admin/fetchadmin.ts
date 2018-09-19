import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { SpecialDate } from '../../models/specialdate';
import { Site } from '../../models/site';

@Component
export default class FetchAbsenceComponent extends Vue {
	specialdates: SpecialDate[] = [];
	sites: Site[] = [];
	loadingSpecialDate: boolean = false;
	loadingSite: boolean = false;
	searchSpecialDate: string = "";
	searchSite: string = "";

	headersSpecialDate: object[] = [
		{ text: 'Name', value: 'name' },
		{ text: 'Date', value: 'date' }
	];

	headersSite: object[] = [
		{ text: 'Name', value: 'name' },
		{ text: 'Type', value: 'type' },
		{ text: 'Times', value: 'times' }
	];

	mounted() {
		this.loadSpecialDates();
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

	createSite() {
		this.$router.push("/createsite");
	}

	editSpecialDate(id: number) {
		console.log(id);
		this.$router.push("/editspecialdate/" + id);
	}

	editSite(id: number) {
		console.log(id);
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
