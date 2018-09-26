import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import { Employee } from '../../models/employee';
import { SelectedDate } from '../../models/selecteddate';

@Component
export default class FetchEmployeeComponent extends Vue {
	@Prop(SelectedDate) selecteddate!: SelectedDate;
	employees: Employee[] = [];
	date: string = "";
	loading: boolean = false;
	search: string = "";
	headers: object[] = [
		{ text: 'Id', value: 'id' },
		{ text: 'Name', value: 'name' },
		{ text: 'Role', value: 'role' },
		{ text: 'Skill', value: 'skill' },
		{ text: 'Address', value: 'address' },
		{ text: 'Number', value: 'number' },
		{ text: 'Contract Hours', value: 'contractHours' },
		{ text: 'Work Pattern', value: 'workPattern' },
		{ text: 'Status', value: 'status' },
	];

	mounted() {
		this.loadEmployees(t);
	}

	loadEmployees() {
		this.loading = true;
		fetch('api/Employee/GetEmployees?date=' + this.selecteddate.date)
			.then(response => response.json() as Promise<Employee[]>)
			.then(data => {
				this.employees = data;
				this.loading = false;
			});
	}
	
	statusColour(type: string) {
		switch (type) {
			case "Okay":
				return "Black";
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

	createEmployee() {
		this.$router.push("/createemployee");
	}

	editEmployee(id: number) {
		this.$router.push("/editemployee/" + id);
	}
	
	viewEmployee(id: number) {
		this.$router.push("/viewemployee/" + id);
	}

	deleteEmployee(id: number) {
		var ans = confirm("Do you want to delete Employee " + id + "?");
		if (ans) {
			fetch('api/Employee/Delete?id=' + id, {
				method: 'DELETE'
			})
				.then(response => response.json() as Promise<number>)
				.then(data => {
					if (data < 1) {
						alert("Failed to delete employee. Please make sure you are still connected.");
					} else {
						this.loadEmployees();
					}
				})
		}
	}
}
