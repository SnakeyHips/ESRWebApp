import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Session } from '../../models/session';
import { Employee } from '../../models/employee';

@Component
export default class ViewEmployeeComponent extends Vue {
	sessions: Session[] = [];
	startdate: string = "";
	enddate: string = "";
	startDateFormatted = "";
	endDateFormatted = "";
	loading: boolean = false;
	failed: boolean = false;
	search: string = "";
	headers: object[] = [
		{ text: 'Day', value: 'day' },
		{ text: 'Date', value: 'date' },
		{ text: 'Site', value: 'site' },
		{ text: 'Time', value: 'time' },
		{ text: 'Chairs', value: 'chairs' },
		{ text: 'OCC', value: 'occ' },
		{ text: 'Estimate', value: 'estimate' },
	];

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

	mounted() {
		this.loading = true;
		fetch('api/Employee/GetById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Employee>)
			.then(data => {
				this.employee = data;
				this.loading = false;
			});
	}

	loadSessions() {
		if (this.startdate != "") {
			this.startDateFormatted = new Date(this.startdate).toLocaleDateString();
		}
		if (this.enddate != "") {
			this.endDateFormatted = new Date(this.enddate).toLocaleDateString();
		}
		if (this.startdate != "" && this.enddate != "") {
			if (this.enddate >= this.startdate) {
				this.loading = true;
				fetch('api/Session/GetEmployeeSessions?staffid=' + this.employee.id + '&startdate=' + this.startdate + '&enddate=' + this.enddate)
					.then(response => response.json() as Promise<Session[]>)
					.then(data => {
						this.sessions = data;
						this.failed = false;
						this.loading = false;
					});
			} else {
				this.sessions = [];
				this.failed = true;
			}
		}
	}
}
