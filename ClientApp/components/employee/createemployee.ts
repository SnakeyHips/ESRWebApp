import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Employee } from '../../models/employee';

@Component
export default class CreateEmployeeComponent extends Vue {	

	employee: Employee = {
		id: 0,
		name: "",
		role: "",
		skill: "",
		address: "",
		number: "",
		contractHours: 0.0,
		appointedHours: 0.0,
		absenceHours: 0.0,
		lowRateUHours: 0.0,
		highRateUHours: 0.0,
		overTimeHours: 0.0,
		workPattern: "",
		status: ""
	}

	createEmployee() {
		fetch('api/Employee/Create', {
			method: 'POST',
			body: JSON.stringify(this.employee)
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					alert("Failed to create Employee. Please make sure the Id is not already in use.");
				} else {
					this.$router.push('/fetchemployee');
				}
			})
	}
}