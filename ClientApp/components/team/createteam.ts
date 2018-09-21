import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Team } from '../../models/team';
import { Employee } from '../../models/employee';
import { VuetifyObject } from 'vuetify';

@Component
export default class CreateTeamComponent extends Vue {	
	$refs!: {
		form: HTMLFormElement,
		sv1: VuetifyObject,
		dri1: VuetifyObject,
		dri2: VuetifyObject,
		rn1: VuetifyObject,
		rn2: VuetifyObject,
		rn3: VuetifyObject,
		cca1: VuetifyObject,
		cca2: VuetifyObject,
		cca3: VuetifyObject
	}

	rules: object = {
		required: value => !!value || 'Required',
		number: value => /[0-9]/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: value => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	team: Team = {
		id: 0,
		name: "",
		sV1Id: 0,
		sV1Name: "",
		drI1Id: 0,
		drI1Name: "",
		drI2Id: 0,
		drI2Name: "",
		rN1Id: 0,
		rN1Name: "",
		rN2Id: 0,
		rN2Name: "",
		rN3Id: 0,
		rN3Name: "",
		ccA1Id: 0,
		ccA1Name: "",
		ccA2Id: 0,
		ccA2Name: "",
		ccA3Id: 0,
		ccA3Name: "",
	}

	loading: boolean = false;
	failed: boolean = false;
	errorMessage: string = "";
	employees: Employee[] = [];
	svs: Employee[] = [];
	dris: Employee[] = [];
	rns: Employee[] = [];
	ccas: Employee[] = [];

	mounted() {
		this.loading = true;
		fetch('api/Employee/GetEmployees?date=' + new Date().toISOString().slice(0, 10))
			.then(response => response.json() as Promise<Employee[]>)
			.then(data => {
				this.employees = data;
				this.filterRoles();
				this.loading = false;
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
				case "RN":
					this.rns.push(this.employees[i]);
					break;
				case "CCA":
					this.ccas.push(this.employees[i]);
					break;
			}
		}
	}

	//Check for duplicates selected
	checkDuplicates() {
		let duplicate: boolean = false;
		if (this.team.drI1Id > 0) {
			if (this.team.drI2Id > 0) {
				if (this.team.drI1Id === this.team.drI2Id) {
					this.errorMessage = "Duplicate driver 1 and 2 found!";
					this.failed = true;
					duplicate = true;
				}
			}
		}
		if (this.team.rN1Id > 0) {
			if (this.team.rN2Id > 0) {
				if (this.team.rN1Id === this.team.rN2Id) {
					this.errorMessage = "Duplicate RN 1 and 2 found!";
					this.failed = true;
					duplicate = true;
				}
			}
			if (this.team.rN3Id > 0) {
				if (this.team.rN1Id === this.team.rN3Id) {
					this.errorMessage = "Duplicate RN 1 and 3 found!";
					this.failed = true;
					duplicate = true;
				}
			}
		}
		if (this.team.rN2Id > 0) {
			if (this.team.rN2Id === this.team.rN3Id) {
				this.errorMessage = "Duplicate RN 2 and 3 found!";
				this.failed = true;
				duplicate = true;
			}
		}
		if (this.team.ccA1Id > 0) {
			if (this.team.ccA2Id > 0) {
				if (this.team.ccA1Id === this.team.ccA2Id) {
					this.errorMessage = "Duplicate CCA 1 and 2 found!";
					this.failed = true;
					duplicate = true;
				}
			}
			if (this.team.ccA3Id > 0) {
				if (this.team.ccA1Id === this.team.ccA3Id) {
					this.errorMessage = "Duplicate CCA 1 and 3 found!";
					this.failed = true;
					duplicate = true;
				}
			}
		}
		if (this.team.ccA2Id > 0) {
			if (this.team.ccA2Id === this.team.ccA3Id) {
				this.errorMessage = "Duplicate CCA 2 and 3 found!";
				this.failed = true;
				duplicate = true;
			}
		}
		return duplicate;
	}

	createTeam() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			if (!this.checkDuplicates()) {
				this.getNames();
				fetch('api/Team/Create', {
					method: 'POST',
					body: JSON.stringify(this.team)
				})
					.then(response => response.json() as Promise<number>)
					.then(data => {
						if (data < 1) {
							this.errorMessage = "Failed to create Team!";
							this.failed = true;
						} else {
							this.$router.push('/fetchteam');
						}
					})
			}
		}
	}

	//Set name methods
	getNames() {
		try {
			this.team.sV1Name = this.$refs.sv1.$data.selectedItems[0].name;
		} catch{
			this.team.sV1Name = "";
		}
		try {
			this.team.drI1Name = this.$refs.dri1.$data.selectedItems[0].name;
		} catch{
			this.team.drI1Name = "";
		}
		try {
			this.team.drI2Name = this.$refs.dri2.$data.selectedItems[0].name;
		} catch{
			this.team.drI2Name = "";
		}
		try {
			this.team.rN1Name = this.$refs.rn1.$data.selectedItems[0].name;
		} catch{
			this.team.rN1Name = "";
		}
		try {
			this.team.rN2Name = this.$refs.rn2.$data.selectedItems[0].name;
		} catch{
			this.team.rN2Name = "";
		}
		try {
			this.team.rN3Name = this.$refs.rn3.$data.selectedItems[0].name;
		} catch{
			this.team.rN3Name = "";
		}
		try {
			this.team.ccA1Name = this.$refs.cca1.$data.selectedItems[0].name;
		} catch{
			this.team.ccA1Name = "";
		}
		try {
			this.team.ccA2Name = this.$refs.cca2.$data.selectedItems[0].name;
		} catch{
			this.team.ccA2Name = "";
		}
		try {
			this.team.ccA3Name = this.$refs.cca3.$data.selectedItems[0].name;
		} catch{
			this.team.ccA3Name = "";
		}
	}

	clear() {
		this.$refs.form.reset();
	}

	cancel() {
		this.$router.push('/fetchteam');
	}
}