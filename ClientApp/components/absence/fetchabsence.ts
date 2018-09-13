import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Absence } from '../../models/absence';

@Component
export default class FetchAbsenceComponent extends Vue {
	absences: Absence[] = [];
	date: string = "";
	mount: boolean = false;

	mounted() {
		this.date = new Date().toISOString().slice(0, 10);
		this.loadAbsences(this.date);
	}

	loadAbsences(date: string) {
		this.mount = false;
		fetch('api/Absence/GetAbsences?date=' + date)
			.then(response => response.json() as Promise<Absence[]>)
			.then(data => {
				this.absences = data;
				this.mount = true;
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
