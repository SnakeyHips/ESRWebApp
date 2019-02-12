import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Absence } from '../../models/absence';
import { Employee } from '../../models/employee';
import { AbsenceType } from '../../models/absencetype';

@Component
export default class CreateAbsenceComponent extends Vue {	
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: (value: string) => !!value || 'Required',
		number: (value: string) => /^\d+(\d{1,2})?$/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: (value: string) => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	absence: Absence = {
		id: 0,
		employeeId: 0,
		employeeName: "",
		type: "",
		startDate: "",
		endDate: "",
		partDay: "",
		hours: 0
	}

	startDateFormatted = "";
	endDateFormatted = "";
	iddisable: boolean = false;
	loading: boolean = false;
	failed: boolean = false;
	errormessage: string = "";
	absencetypes: AbsenceType[] = [];
	employees: Employee[] = [];
	partDays: string[] = ["Yes", "No"];

	mounted() {
		this.loading = true;
		this.loadAbsenceTypes();
		this.loadEmployees();
		this.loading = false;
	}

	loadEmployees() {
		fetch('api/Employee/GetEmployees')
			.then(response => response.json() as Promise<Employee[]>)
			.then(data => {
				this.employees = data;
			});
	}

	loadAbsenceTypes() {
		fetch('api/Admin/GetAbsenceTypes')
			.then(response => response.json() as Promise<AbsenceType[]>)
			.then(data => {
				this.absencetypes = data;
			});
	}

	customFilter(item: Employee, queryText: string, itemText: string) {
		// Search via the Employee Id/Name rather than itemText as yields better results
		const idText = item.id.toString().toLowerCase();
		const nameText = item.name.toLowerCase();
		return idText.indexOf(queryText.toLowerCase()) > -1 || nameText.indexOf(queryText.toLowerCase()) > -1;
	}

	createAbsence() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			if (this.absence.endDate >= this.absence.startDate) {
				fetch('api/Absence/Create', {
					method: 'POST',
					body: JSON.stringify(this.absence)
				})
					.then(response => response.json() as Promise<number>)
					.then(data => {
						if (data < 1) {
							this.errormessage = "Failed to create absence!";
							this.failed = true;
						} else {
							this.$router.push('/fetchabsence');
						}
					})
			} else {
				this.failed = true;
			}
		}
	}

	formatStartDate() {
		this.startDateFormatted = new Date(this.absence.startDate).toLocaleDateString();
	}

	formatEndDate() {
		this.endDateFormatted = new Date(this.absence.endDate).toLocaleDateString();
	}

	clear() {
		this.iddisable = false;
		this.$refs.form.reset();
	}

	cancel() {
		this.$router.push('/fetchabsence');
	}
}