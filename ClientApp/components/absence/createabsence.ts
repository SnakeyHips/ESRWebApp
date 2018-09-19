import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Absence } from '../../models/absence';
import { Employee } from '../../models/employee';

@Component
export default class CreateAbsenceComponent extends Vue {	
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: value => !!value || 'Required',
		number: value => /[0-9]/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: value => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	absence: Absence = {
		id: 0,
		staffId: 0,
		staffName: "",
		type: "",
		startDate: "",
		endDate: "",
		hours: 0
	}

	failed: boolean = false;
	types: string[] = ["Day Off", "Annual Leave", "Sick Leave", "Special Leave", "Training"];

	createAbsence() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			if (this.checkDates()) {
				fetch('api/Absence/Create', {
					method: 'POST',
					body: JSON.stringify(this.absence)
				})
					.then(response => response.json() as Promise<number>)
					.then(data => {
						if (data < 1) {
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

	searchById(staffId: number) {
		fetch('api/Employee/GetById?id=' + staffId)
			.then(response => response.json() as Promise<Employee>)
			.then(data => {
				if (data != null) {
					this.absence.staffName = data.name;
				} else {
					alert("Couldn't find Employee by that Id!");
				}
			})
	}

	checkDates() {
		if (new Date(this.absence.endDate) < new Date(this.absence.startDate)) {
			return false;
		} else {
			return true;
		}
	}

	clear() {
		this.$refs.form.reset();
	}

	cancel() {
		this.$router.push('/fetchabsence');
	}
}
