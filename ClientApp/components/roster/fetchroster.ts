import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Employee } from '../../models/employee';

@Component
export default class FetchRosterComponent extends Vue {
	weeks: number[] = [];
	employees: Employee[] = [];
	selectedWeeks: number[] = [];
	mount: boolean = false;

	mounted() {
		this.loadWeeks();
	}

	loadWeeks() {
		fetch('api/Roster/GetRosterWeeks')
			.then(response => response.json() as Promise<number[]>)
			.then(data => {
				this.weeks = data;
			})
	}

	loadRoster(selectedWeeks: number[]) {
		this.mount = false;
		console.log(JSON.stringify(selectedWeeks));
		fetch('api/Roster/GetRoster', {
			method: 'POST',
			body: JSON.stringify(selectedWeeks)
		})
			.then(response => response.json() as Promise<Employee[]>)
			.then(data => {
				this.employees = data;
				this.mount = true;
			});
	}

	viewRoster(id: number) {
		console.log(id);
	}
}
