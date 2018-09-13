import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Absence } from '../../models/absence';
import { Employee } from '../../models/employee';

@Component
export default class CreateAbsenceComponent extends Vue {	

	absence: Absence = {
		id: 0,
		staffId: 0,
		staffName: "",
		type: "",
		startDate: "",
		endDate: "",
		hours: 0
	}

	createAbsence() {
		fetch('api/Absence/Create', {
			method: 'POST',
			body: JSON.stringify(this.absence)
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					alert("Failed to create Absence. Please make sure all of the fields are completed.");
				} else {
					this.$router.push('/fetchabsence');
				}
			})
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
}
