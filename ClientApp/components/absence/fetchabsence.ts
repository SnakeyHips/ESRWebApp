import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Absence } from '../../models/absence';

@Component
export default class FetchAbsenceComponent extends Vue {
	absences: Absence[] = [];
	date: string = "";
	loading: boolean = false;
	search: string = "";
	headers: object[] = [
		{ text: 'Staff Id', value: 'staffId' },
		{ text: 'Staff Name', value: 'staffName' },
		{ text: 'Type', value: 'type' },
		{ text: 'Start Date', value: 'startDate' },
		{ text: 'End Date', value: 'endDate' },
		{ text: 'Hours', value: 'hours' },
	];

	mounted() {
		this.loadAbsences(this.date);
	}

	loadAbsences(date: string) {
		this.loading = true;
		fetch('api/Absence/GetAbsences?date=' + date)
			.then(response => response.json() as Promise<Absence[]>)
			.then(data => {
				this.absences = data;
				this.loading = false;
			});
	}

	createAbsence() {
		this.$router.push("/createabsence");
	}

	editAbsence(id: number) {
		this.$router.push("/editabsence/" + id);
	}

	deleteAbsence(id: number) {
		var ans = confirm("Do you want to delete Absence " + id + "?");
		if (ans) {
			fetch('api/Absence/Delete?id=' + id, {
				method: 'DELETE'
			})
				.then(response => response.json() as Promise<number>)
				.then(data => {
					if (data < 1) {
						alert("Failed to delete absence. Please make sure you are still connected?");
					} else {
						this.loadAbsences(this.date);
					}
				})
		}
	}
}
