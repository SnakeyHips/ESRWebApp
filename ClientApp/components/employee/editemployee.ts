import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Employee } from '../../models/employee';

@Component
export default class EditEmployeeComponent extends Vue {
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: value => !!value || 'Required',
		number: value => /[0-9]/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: value => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

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
		negHours: 0.0,
		coHours: 0.0,
		workPattern: "",
		status: ""
	}

	loading: boolean = false;
	failed: boolean = false;
	skills: string[] = [];
	workpattern: string[] = [];
	roles: string[] = ["SV", "DRI", "RN", "CCA"];
	days: string[] = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];


	mounted() {
		this.loading = true;
		fetch('api/Employee/GetById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Employee>)
			.then(data => {
				this.employee = data;
				this.workpattern = this.employee.workPattern.split(",");
				this.loadSkills();
				this.loading = false;
			});
	}

	editEmployee() {
		this.failed = false;
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
					this.failed = true;
				} else {
					this.$router.push('/fetchemployee');
				}
			})
	}

	loadSkills() {
		fetch('api/Admin/GetSkillsByRole?role=' + this.employee.role)
			.then(response => response.json() as Promise<string[]>)
			.then(data => {
				this.skills = data;
			})
	}

	cancel() {
		this.$router.push('/fetchemployee');
	}
}