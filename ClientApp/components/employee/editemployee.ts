import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Employee } from '../../models/employee';

@Component
export default class EditEmployeeComponent extends Vue {

	mount: boolean = false;
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
		overtimeHours: 0.0,
		workPattern: "",
		status: ""
	}
	workpattern: string[] = [];

	mounted() {
		fetch('api/Employee/GetById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Employee>)
			.then(data => {
				this.employee = data;
				this.workpattern = this.employee.workPattern.split(",");
				this.mount = true;
			});
	}

	editEmployee() {
		//Get work pattern first
		this.employee.workPattern = this.workpattern.join();

		//Then send employee to backend
		fetch('api/Employee/Update', {
			method: 'PUT',
			body: JSON.stringify(this.employee)
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					alert("Failed to edit Employee. Please make sure all fields are correct.");
				} else {
					this.$router.push('/fetchemployee');
				}
			})
	}
}
