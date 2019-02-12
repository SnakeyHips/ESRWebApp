import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Team } from '../../models/team';
import { Employee } from '../../models/employee';
import { TeamMember } from '../../models/teammember';

@Component
export default class EditTeamComponent extends Vue {
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: (value: string) => !!value || 'Required',
		number: (value: string) => /^\d+(\d{1,2})?$/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: (value: string) => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	team: Team = {
		id: 0,
		name: "",
		members: []
	}

	loading: boolean = false;
	failed: boolean = false;
	errorMessage: string = "";
	roles: string[] = [];
	employees: Employee[] = [];
	members: TeamMember[] = [];

	mounted() {
		this.loading = true;
		fetch('api/Employee/GetEmployees')
			.then(response => response.json() as Promise<Employee[]>)
			.then(data => {
				this.employees = data;
				this.loadRoles();
				this.loadTeam();
				this.loading = false;
			});
	}

	loadTeam() {
		fetch('api/Team/GetById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Team>)
			.then(data => {
				this.team = data;
				this.members = this.team.members;
				this.loading = false;
			});
	}

	loadRoles() {
		fetch('api/Admin/GetRoleNames')
			.then(response => response.json() as Promise<string[]>)
			.then(data => {
				this.roles = data;
			})
	}

	createTeamMember(role: string) {
		var temp: TeamMember = {
			id: 0,
			teamId: 0,
			employeeId: 0,
			employeeName: "",
			employeeRole: role
		};
		return temp;
	}

	addMember() {
		if (this.members.length < 20) {
			this.members.push(this.createTeamMember(''));
		}
	}

	removeMember() {
		if (this.members.length > 1) {
			this.members.pop();
		}
	}

	customFilter(item: Employee, queryText: string, itemText: string) {
		// Search via the Member Id/Name rather than itemText as yields better results
		const idText = item.id.toString().toLowerCase();
		const nameText = item.name.toLowerCase();
		return idText.indexOf(queryText.toLowerCase()) > -1 || nameText.indexOf(queryText.toLowerCase()) > -1;
	}

	//Check for duplicates selected
	checkDuplicates() {
		this.failed = false;
		var duplicate: boolean = false;
		for (var i = 0; i < this.members.length; i++) {
			if (this.members[i].employeeId > 0) {
				for (var j = 0; j < this.members.length; j++) {
					if (this.members[j].employeeId > 0) {
						if (this.members[i] != this.members[j]) {
							if (this.members[i].employeeId === this.members[j].employeeId) {
								this.failed = true;
								this.errorMessage = "Duplicate member assigned!";
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

	editTeam() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			if (!this.checkDuplicates()) {
				this.team.members = [];
				this.team.members = this.members;
				fetch('api/Team/Update', {
					method: 'PUT',
					body: JSON.stringify(this.team)
				})
					.then(response => response.json() as Promise<number>)
					.then(data => {
						if (data < 1) {
							this.errorMessage = "Failed to update Team!";
							this.failed = true;
						} else {
							this.$router.push('/fetchteam');
						}
					})
			}
		}
	}

	clear() {
		this.members = [];
		this.members.push(this.createTeamMember(''));
	}

	cancel() {
		this.$router.push('/fetchteam');
	}
}