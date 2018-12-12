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
		staffCount: 0,
		state: 0,
		employees: []
	}

	team: Team = {
		id: 0,
		name: "",
		members: []
	}

	employees: Employee[] = [];
	teams: Team[] = [];
	svs: Employee[] = [];
	dris: Employee[] = [];
	rns: Employee[] = [];
	ccas: Employee[] = [];
	sessionsvs: SessionEmployee[] = [];
	sessiondris: SessionEmployee[] = [];
	sessionccas: SessionEmployee[] = [];
	sessionrns: SessionEmployee[] = [];

	mounted() {
		this.loading = true;
		//Get session first
		fetch('api/Session/GetById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Session>)
			.then(data => {
				this.before = JSON.parse(JSON.stringify(data));
				this.after = data;
				if (this.after.holiday > 0) {
					this.holiday = true;
				}
				this.filterSessionRoles();
				//then get available and teams
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

	addSV() {
		if (this.sessionsvs.length < 2) {
			this.sessionsvs.push(this.createSessionEmployee("SV"));
		}
	}

	addDRI() {
		if (this.sessiondris.length < 2) {
			this.sessiondris.push(this.createSessionEmployee("DRI"));
		}
	}
	addCCA() {
		if (this.sessionccas.length < 5) {
			this.sessionccas.push(this.createSessionEmployee("CCA"));
		}
	}

	addRN() {
		if (this.sessionrns.length < 5) {
			this.sessionrns.push(this.createSessionEmployee("RN"));
		}
	}

	removeSV() {
		if (this.sessionsvs.length > 1) {
			this.sessionsvs.pop();
		}
	}

	removeDRI() {
		if (this.sessiondris.length > 1) {
			this.sessiondris.pop();
		}
	}

	removeCCA() {
		if (this.sessionccas.length > 1) {
			this.sessionccas.pop();
		}
	}

	removeRN() {
		if (this.sessionrns.length > 1) {
			this.sessionrns.pop();
		}
	}

	loadAvailable() {
		fetch('api/Employee/GetAvailable?date=' + this.before.date + "&day=" + this.before.day)
			.then(response => response.json() as Promise<Employee[]>)
			.then(data => {
				this.employees = data;
				this.filterRoles();
			});
	}

	loadTeams() {
		fetch('api/Team/GetTeams')
			.then(response => response.json() as Promise<Team[]>)
			.then(data => {
				this.teams = data;
			});
	}

	filterRoles() {
		for (var i = 0; i < this.employees.length; i++) {
			switch (this.employees[i].role) {
				case "SV":
					this.svs.push(this.employees[i]);
					break;
				case "DRI":
					this.dris.push(this.employees[i]);
					break;
				case "CCA":
					this.ccas.push(this.employees[i]);
					break;
				case "RN":
					this.rns.push(this.employees[i]);
					break;
			}
		}
	}

	filterSessionRoles() {
		for (var i = 0; i < this.after.employees.length; i++) {
			switch (this.after.employees[i].employeeRole) {
				case "SV":
					this.sessionsvs.push(this.after.employees[i]);
					break;
				case "DRI":
					this.sessiondris.push(this.after.employees[i]);
					break;
				case "CCA":
					this.sessionccas.push(this.after.employees[i]);
					break;
				case "RN":
					this.sessionrns.push(this.after.employees[i]);
					break;
			}
		}
		if (this.sessionsvs.length < 1) {
			this.sessionsvs.push(this.createSessionEmployee("SV"));
		}
		if (this.sessiondris.length < 1) {
			this.sessiondris.push(this.createSessionEmployee("DRI"));
		}
		if (this.sessionccas.length < 1) {
			this.sessionccas.push(this.createSessionEmployee("CCA"));
		}
		if (this.sessionrns.length < 1) {
			this.sessionrns.push(this.createSessionEmployee("RN"));
		}
	}

	setTeam() {
		this.sessionsvs = [];
		this.sessiondris = [];
		this.sessionccas = [];
		this.sessionrns = [];
		for (var i = 0; i < this.team.members.length; i++) {
			if (this.searchTeam(this.team.members[i].employeeId)) {
				switch (this.team.members[i].employeeRole) {
					case "SV":
						this.sessionsvs.push(this.convertSessionEmployee(this.team.members[i]));
						break;
					case "DRI":
						this.sessiondris.push(this.convertSessionEmployee(this.team.members[i]));
						break;
					case "CCA":
						this.sessionccas.push(this.convertSessionEmployee(this.team.members[i]));
						break;
					case "RN":
						this.sessionrns.push(this.convertSessionEmployee(this.team.members[i]));
						break;
				}
			} else {
				switch (this.team.members[i].employeeRole) {
					case "SV":
						this.sessionsvs.push(this.createSessionEmployee("SV"));
						break;
					case "DRI":
						this.sessiondris.push(this.createSessionEmployee("DRI"));
						break;
					case "CCA":
						this.sessionccas.push(this.createSessionEmployee("CCA"));
						break;
					case "RN":
						this.sessionrns.push(this.createSessionEmployee("RN"));
						break;
				}
			}
		}
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
		for (var i = 0; i < this.sessionsvs.length - 1; i++) {
			if (this.sessionsvs[i + 1].employeeId == this.sessionsvs[i].employeeId) {
				this.failed = true;
				this.errorMessage = "Duplicate SV found!";
				duplicate = true;
				break;
			}
		}
		for (var i = 0; i < this.sessiondris.length - 1; i++) {
			if (this.sessiondris[i + 1].employeeId == this.sessiondris[i].employeeId) {
				this.failed = true;
				this.errorMessage = "Duplicate DRI found!";
				duplicate = true;
				break;
			}
		}
		for (var i = 0; i < this.sessionccas.length - 1; i++) {
			if (this.sessionccas[i + 1].employeeId == this.sessionccas[i].employeeId) {
				this.failed = true;
				this.errorMessage = "Duplicate CCA found!";
				duplicate = true;
				break;
			}
		}
		for (var i = 0; i < this.sessionrns.length - 1; i++) {
			if (this.sessionrns[i + 1].employeeId == this.sessionrns[i].employeeId) {
				this.failed = true;
				this.errorMessage = "Duplicate RN found!";
				duplicate = true;
				break;
			}
		}
		return duplicate;
	}

	populateEmployees() {
		this.after.employees = [];
		for (var i = 0; i < this.sessionsvs.length; i++) {
			if (this.sessionsvs[i].employeeId > 0) {
				this.after.employees.push(this.sessionsvs[i]);
			}
		}
		for (var i = 0; i < this.sessiondris.length; i++) {
			if (this.sessiondris[i].employeeId > 0) {
				this.after.employees.push(this.sessiondris[i]);
			}
		}
		for (var i = 0; i < this.sessionccas.length; i++) {
			if (this.sessionccas[i].employeeId > 0) {
				this.after.employees.push(this.sessionccas[i]);
			}
		}
		for (var i = 0; i < this.sessionrns.length; i++) {
			if (this.sessionrns[i].employeeId > 0) {
				this.after.employees.push(this.sessionrns[i]);
			}
		}
	}

	rosterSession() {
		this.failed = false;
		this.loading = true;
		if (!this.checkDuplicates()) {
			this.populateEmployees();
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
		this.sessionsvs = [];
		this.sessionsvs.push(this.createSessionEmployee("SV"));
		this.sessiondris = [];
		this.sessiondris.push(this.createSessionEmployee("DRI"));
		this.sessionccas = [];
		this.sessionccas.push(this.createSessionEmployee("CCA"));
		this.sessionrns = [];
		this.sessionrns.push(this.createSessionEmployee("RN"));
	}

	cancel() {
		this.$router.push('/fetchsession');
	}
}