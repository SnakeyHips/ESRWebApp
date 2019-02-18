import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Session } from '../../models/session';
import { Employee } from '../../models/employee';
import { Team } from '../../models/team';
import { SessionEmployee } from '../../models/sessionemployee';
import { TeamMember } from '../../models/teammember';

@Component
export default class RosterSessionComponent extends Vue {

	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: (value: string) => !!value || 'Required',
		number: (value: string) => /^\d+(\d{1,2})?$/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: (value: string) => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	test: string = "";
	failed: boolean = false;
	errorMessage: string = "";
	loading: boolean = false;
	holiday: boolean = false;
	roles: string[] = [];
	employees: Employee[] = [];
	teams: Team[] = [];

	before: Session = {
		id: 0,
		date: "",
		day: "",
		type: "",
		site: "",
		time: "",
		lod: 0,
		chairs: 0,
		occ: 0,
		estimate: 0,
		holiday: 0,
		note: "",
		template: "",
		staffCount: 0,
		state: 0,
		employees: []
	}

	after: Session = {
		id: 0,
		date: "",
		day: "",
		type: "",
		site: "",
		time: "",
		lod: 0,
		chairs: 0,
		occ: 0,
		estimate: 0,
		holiday: 0,
		note: "",
		template: "",
		staffCount: 0,
		state: 0,
		employees: []
	}

	team: Team = {
		id: 0,
		name: "",
		members: []
	}



	mounted() {
		this.loading = true;
		//Get session first
		fetch('api/Session/GetByIdRoster?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Session>)
			.then(data => {
				this.before = JSON.parse(JSON.stringify(data));
				this.after = data;
				if (this.after.holiday > 0) {
					this.holiday = true;
				}
				//then get relevant data for rostering
				this.loadRoles();
				this.loadAvailable();
				this.loadTeams();
				this.loading = false;
			});
	}

	createSessionEmployee(role: string) {
		var temp: SessionEmployee = {
			id: 0,
			sessionId: this.after.id,
			sessionDate: this.after.date,
			sessionSite: this.after.site,
			employeeId: 0,
			employeeName: "",
			employeeRole: role,
			employeeLOD: this.after.lod,
			employeeUNS: 0.0,
			employeeOT: 0.0
		};
		return temp;
	}

	convertSessionEmployee(member: TeamMember) {
		var temp: SessionEmployee = {
			id: 0,
			sessionId: this.after.id,
			sessionDate: this.after.date,
			sessionSite: this.after.site,
			employeeId: member.employeeId,
			employeeName: member.employeeName,
			employeeRole: member.employeeRole,
			employeeLOD: this.after.lod,
			employeeUNS: 0.0,
			employeeOT: 0.0
		};
		return temp;
	}

	addEmployee() {
		if (this.after.employees.length < 20) {
			this.after.employees.push(this.createSessionEmployee(''));
		}
	}

	removeEmployee() {
		if (this.after.employees.length > 1) {
			this.after.employees.pop();
		}
	}

	loadRoles() {
		fetch('api/Admin/GetRoleNames')
			.then(response => response.json() as Promise<string[]>)
			.then(data => {
				this.roles = data;
			})
	}

	loadTemplate() {
		this.loading = true;
		this.after.employees = [];
		fetch('api/Admin/GetTemplateByName?name=' + this.after.template)
			.then(response => response.json() as Promise<string>)
			.then(data => {
				let templateRoles: string[] = data.split(',');
				for (var i = 0; i < templateRoles.length; i++) {
					this.after.employees.push(this.createSessionEmployee(templateRoles[i]));
				}
				this.loading = false;
			});
	}

	loadAvailable() {
		fetch('api/Employee/GetAvailable?date=' + this.before.date + "&day=" + this.before.day)
			.then(response => response.json() as Promise<Employee[]>)
			.then(data => {
				this.employees = data;
			});
	}

	loadTeams() {
		fetch('api/Team/GetTeams')
			.then(response => response.json() as Promise<Team[]>)
			.then(data => {
				this.teams = data;
			});
	}

	populateTeam() {
		// Replaces Template with Team
		this.after.employees = [];
		for (var i = 0; i < this.team.members.length; i++) {
			if (this.searchTeam(this.team.members[i].employeeId)) {
				this.after.employees.push(this.convertSessionEmployee(this.team.members[i]));
			}
		}
	}

	customFilter(item: Employee, queryText: string, itemText: string) {
		// Search via the Employee Id/Name rather than itemText as yields better results
		const idText = item.id.toString().toLowerCase();
		const nameText = item.name.toLowerCase();
		const skillText = item.skill.toLowerCase();
		return idText.indexOf(queryText.toLowerCase()) > -1 || nameText.indexOf(queryText.toLowerCase()) > -1 || skillText.indexOf(queryText.toLowerCase()) > -1;
	}

	searchTeam(id: number) {
		let match: boolean = false;
		for (var i = 0; i < this.employees.length; i++) {
			if (this.employees[i].id === id) {
				match = true;
				break
			}
		}
		return match;
	}

	checkDuplicates() {
		this.failed = false;
		var duplicate: boolean = false;
		for (var i = 0; i < this.after.employees.length; i++) {
			if (this.after.employees[i].employeeId > 0) {
				for (var j = 0; j < this.after.employees.length; j++) {
					if (this.after.employees[j].employeeId > 0) {
						if (this.after.employees[i] != this.after.employees[j]) {
							if (this.after.employees[i].employeeId === this.after.employees[j].employeeId) {
								this.failed = true;
								this.errorMessage = "Duplicate employee rostered!";
								duplicate = true;
								break;
							}
						}
					}
				}
			}
		}
		return duplicate;
	}

	rosterSession() {
		this.failed = false;
		this.loading = true;
		// Then check for duplicates
		if (!this.checkDuplicates()) {
			let sessions: Session[] = [];
			sessions.push(this.before);
			sessions.push(this.after);
			fetch('api/Roster/Update', {
				method: 'PUT',
				body: JSON.stringify(sessions)
			})
				.then(response => response.json() as Promise<number>)
				.then(data => {
					if (data < 1) {
						this.errorMessage = "Failed to roster session!";
						this.failed = true;
						this.loading = false;
					} else {
						this.$router.push('/fetchsession');
					}
				})
		} else {
			this.loading = false;
		}
	}

	clear() {
		this.team = {
			id: 0,
			name: "",
			members: []
		}	
		this.loadTemplate();
	}

	cancel() {
		this.$router.push('/fetchsession');
	}
}