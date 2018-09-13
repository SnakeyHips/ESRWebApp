import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Absence } from '../../models/absence';

@Component
export default class EditAbsenceComponent extends Vue {

	mount: boolean = false;

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

	mounted() {
		fetch('api/Absence/GetById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Absence>)
			.then(data => {
				this.before = JSON.parse(JSON.stringify(data));
				this.after = data;
				this.mount = true;
			});
	}

	editAbsence() {
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
					alert("Failed to edit Absence. Please make sure all fields are correct.");
				} else {
					this.$router.push('/fetchabsence');
				}
			})
	}
}
