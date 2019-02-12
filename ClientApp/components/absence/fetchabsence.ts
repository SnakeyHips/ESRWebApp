import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import { Absence } from '../../models/absence';
import { SelectedDate } from '../../models/selecteddate';
import { AbsenceType } from '../../models/absencetype';

@Component
export default class FetchAbsenceComponent extends Vue {
	@Prop(SelectedDate) selecteddate!: SelectedDate;
	dateFormatted: string = new Date(this.selecteddate.date).toLocaleDateString();
	absences: Absence[] = [];
	loading: boolean = false;
	search: string = "";
	failed: boolean = false;
	dialog: boolean = false;
	absencetypes: AbsenceType[] = [];
	headers: object[] = [
		{ text: 'Employee Id', value: 'employeeId' },
		{ text: 'Employee Name', value: 'employeeName' },
		{ text: 'Type', value: 'type' },
		{ text: 'Start Date', value: 'startDate' },
		{ text: 'End Date', value: 'endDate' },
		{ text: 'Part Day', value: 'partDate' },
		{ text: 'Hours', value: 'hours' },
	];

	selected: Absence = {
		id: 0,
		employeeId: 0,
		employeeName: "",
		type: "",
		startDate: "",
		endDate: "",
		partDay: "",
		hours: 0
	}

	mounted() {
		this.loadAbsences();
		this.loadAbsenceTypes();
	}

	loadAbsences() {
		this.loading = true;
		this.dateFormatted = new Date(this.selecteddate.date).toLocaleDateString();
		fetch('api/Absence/GetAbsences?date=' + this.selecteddate.date)
			.then(response => response.json() as Promise<Absence[]>)
			.then(data => {
				this.absences = data;
				this.loading = false;
			});
	}

	loadAbsenceTypes() {
		fetch('api/Admin/GetAbsenceTypes')
			.then(response => response.json() as Promise<AbsenceType[]>)
			.then(data => {
				this.absencetypes = data;
			});
	}

	typeColour(type: string) {
		for (var i = 0; i < this.absencetypes.length; i++) {
			if (this.absencetypes[i].name === type) {
				return this.absencetypes[i].colour;
			}
		}
	}

	dateFormat(date: string) {
		return new Date(date).toLocaleDateString();
	}

	createAbsence() {
		this.$router.push("/createabsence");
	}

	editAbsence(id: number) {
		this.$router.push("/editabsence/" + id);
	}

	openDelete(selected: Absence) {
		this.selected = selected;
		this.dialog = true;
	}

	deleteAbsence() {
		this.failed = false;
		this.dialog = false;
		fetch('api/Absence/Delete', {
			method: 'DELETE',
			body: JSON.stringify(this.selected)
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					this.failed = true;
				} else {
					this.loadAbsences();
				}
			})
	}
}
