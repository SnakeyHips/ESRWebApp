import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import { Absence } from '../../models/absence';
import { SelectedDate } from '../../models/selecteddate';

@Component
export default class FetchAbsenceComponent extends Vue {
	@Prop(SelectedDate) selecteddate!: SelectedDate;
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

	loadAbsences() {
		this.loading = true;
		fetch('api/Absence/GetAbsences?date=' + this.selecteddate.date)
			.then(response => response.json() as Promise<Absence[]>)
			.then(data => {
				this.absences = data;
				this.loading = false;
			});
	}
	
	typeColour(type: string) {
		switch (type) {
			case "Day Off":
				return "LightGray";
			case "Annual Leave":
				return "Plum";
			case "Sick Leave":
				return "LightSeaGreen";
			case "Special Leave":
				return "LightCoral";
			case "Training":
				return "CornflowerBlue";
		}
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
