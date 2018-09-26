import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Absence } from '../../models/absence';

@Component
export default class EditAbsenceComponent extends Vue {
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: value => !!value || 'Required',
		number: value => /[0-9]/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: value => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	before: Absence = {
		id: 0,
		staffId: 0,
		staffName: "",
		type: "",
		startDate: "",
		endDate: "",
		hours: 0
	}

	after: Absence = {
		id: 0,
		staffId: 0,
		staffName: "",
		type: "",
		startDate: "",
		endDate: "",
		hours: 0
	}

	startDateFormatted = "";
	endDateFormatted = "";
	loading: boolean = false;
	failed: boolean = false;
	types: string[] = ["Day Off", "Annual Leave", "Sick Leave", "Special Leave", "Training"];

	mounted() {
		this.loading = true;
		fetch('api/Absence/GetById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Absence>)
			.then(data => {
				this.before = JSON.parse(JSON.stringify(data));
				this.after = data;
				this.formatStartDate();
				this.formatEndDate();
				this.loading = false;
			});
	}

	editAbsence() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			if (this.checkDates()) {
				let absences: Absence[] = [];
				absences.push(this.before);
				absences.push(this.after);
				fetch('api/Absence/Update', {
					method: 'PUT',
					body: JSON.stringify(absences)
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

	formatStartDate() {
		this.startDateFormatted = new Date(this.after.startDate).toLocaleDateString();
	}

	formatEndDate() {
		this.endDateFormatted = new Date(this.after.endDate).toLocaleDateString();
	}

	checkDates() {
		if (new Date(this.after.endDate) < new Date(this.after.startDate)) {
			return false;
		} else {
			return true;
		}
	}

	cancel() {
		this.$router.push('/fetchabsence');
	}
}
