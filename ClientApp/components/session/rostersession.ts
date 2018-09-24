import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Session } from '../../models/session';
import { Employee } from '../../models/employee';
import { Team } from '../../models/team';
import { VuetifyObject } from 'vuetify';

@Component
export default class RosterSessionComponent extends Vue {

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

	test: string = "";
	failed: boolean = false;
	errorMessage: string = "";
	mount: boolean = false;
	holiday: boolean = false;
	sv1disable: boolean = true;
	dri1disable: boolean = true;
	dri2selectdisable: boolean = false;
	dri2disable: boolean = true;
	rn1disable: boolean = true;
	rn2selectdisable: boolean = false;
	rn2disable: boolean = true;
	rn3selectdisable: boolean = false;
	rn3disable: boolean = true;
	cca1disable: boolean = true;
	cca2selectdisable: boolean = false;
	cca2disable: boolean = true;
	cca3selectdisable: boolean = false;
	cca3disable: boolean = true;

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
		sV1Id: 0,
		sV1Name: "",
		sV1LOD: 0,
		sV1UNS: 0,
		sV1OT: 0,
		drI1Id: 0,
		drI1Name: "",
		drI1LOD: 0,
		drI1UNS: 0,
		drI1OT: 0,
		drI2Id: 0,
		drI2Name: "",
		drI2LOD: 0,
		drI2UNS: 0,
		drI2OT: 0,
		rN1Id: 0,
		rN1Name: "",
		rN1LOD: 0,
		rN1UNS: 0,
		rN1OT: 0,
		rN2Id: 0,
		rN2Name: "",
		rN2LOD: 0,
		rN2UNS: 0,
		rN2OT: 0,
		rN3Id: 0,
		rN3Name: "",
		rN3LOD: 0,
		rN3UNS: 0,
		rN3OT: 0,
		ccA1Id: 0,
		ccA1Name: "",
		ccA1LOD: 0,
		ccA1UNS: 0,
		ccA1OT: 0,
		ccA2Id: 0,
		ccA2Name: "",
		ccA2LOD: 0,
		ccA2UNS: 0,
		ccA2OT: 0,
		ccA3Id: 0,
		ccA3Name: "",
		ccA3LOD: 0,
		ccA3UNS: 0,
		ccA3OT: 0,
		staffCount: 0,
		state: 0
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
		sV1Id: 0,
		sV1Name: "",
		sV1LOD: 0,
		sV1UNS: 0,
		sV1OT: 0,
		drI1Id: 0,
		drI1Name: "",
		drI1LOD: 0,
		drI1UNS: 0,
		drI1OT: 0,
		drI2Id: 0,
		drI2Name: "",
		drI2LOD: 0,
		drI2UNS: 0,
		drI2OT: 0,
		rN1Id: 0,
		rN1Name: "",
		rN1LOD: 0,
		rN1UNS: 0,
		rN1OT: 0,
		rN2Id: 0,
		rN2Name: "",
		rN2LOD: 0,
		rN2UNS: 0,
		rN2OT: 0,
		rN3Id: 0,
		rN3Name: "",
		rN3LOD: 0,
		rN3UNS: 0,
		rN3OT: 0,
		ccA1Id: 0,
		ccA1Name: "",
		ccA1LOD: 0,
		ccA1UNS: 0,
		ccA1OT: 0,
		ccA2Id: 0,
		ccA2Name: "",
		ccA2LOD: 0,
		ccA2UNS: 0,
		ccA2OT: 0,
		ccA3Id: 0,
		ccA3Name: "",
		ccA3LOD: 0,
		ccA3UNS: 0,
		ccA3OT: 0,
		staffCount: 0,
		state: 0
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

	employees: Employee[] = [];
	teams: Team[] = [];
	svs: Employee[] = [];
	dris: Employee[] = [];
	rns: Employee[] = [];
	ccas: Employee[] = [];

	mounted() {
		//Get session first
		fetch('api/Session/GetById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Session>)
			.then(data => {
				this.before = JSON.parse(JSON.stringify(data));
				this.after = data;
				if (this.after.holiday > 0) {
					this.holiday = true;
				}
				//then get available and teams
				this.loadAvailable();
				this.loadTeams();
				this.loadChairModel();
			});
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

	loadChairModel() {
		if (this.after.chairs < 9) {
			this.rn3selectdisable = true;
			this.cca3selectdisable = true;
			if (this.after.chairs < 6 && this.after.type === "MDC") {
				this.dri2selectdisable = true;
				this.rn2selectdisable = true;
				this.cca2selectdisable = true;
			}
		}
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
		this.mount = true;
	}

	setTeam() {
		this.after.sV1Id = this.team.sV1Id;
		this.after.drI1Id = this.team.drI1Id;
		this.after.drI2Id = this.team.drI2Id;
		this.after.rN1Id = this.team.rN1Id;
		this.after.rN2Id = this.team.rN2Id;
		this.after.rN3Id = this.team.rN3Id;
		this.after.ccA1Id = this.team.ccA1Id;
		this.after.ccA2Id = this.team.ccA2Id;
		this.after.ccA3Id = this.team.ccA3Id;
	}

	rosterSession() {
		this.failed = false;
		if (!this.checkDuplicates()) {
			this.getNames();
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
					} else {
						this.$router.push('/fetchsession');
					}
				})
		}
	}

	sv1select() {
		if (this.after.sV1Id > 0) {
			this.sv1disable = false;
			this.after.sV1LOD = JSON.parse(JSON.stringify(this.after.lod));
		}
	}

	dri1select() {
		if (this.after.drI1Id > 0) {
			this.dri1disable = false;
			this.after.drI1LOD = JSON.parse(JSON.stringify(this.after.lod));
		}
	}

	dri2select() {
		if (this.after.drI2Id > 0) {
			this.dri2disable = false;
			this.after.drI2LOD = JSON.parse(JSON.stringify(this.after.lod));
		}
	}

	rn1select() {
		if (this.after.rN1Id > 0) {
			this.rn1disable = false;
			this.after.rN1LOD = JSON.parse(JSON.stringify(this.after.lod));
		}
	}

	rn2select() {
		if (this.after.rN2Id > 0) {
			this.rn2disable = false;
			this.after.rN2LOD = JSON.parse(JSON.stringify(this.after.lod));
		}
	}

	rn3select() {
		if (this.after.rN3Id > 0) {
			this.rn3disable = false;
			this.after.rN3LOD = JSON.parse(JSON.stringify(this.after.lod));
		}
	}

	cca1select() {
		if (this.after.ccA1Id > 0) {
			this.cca1disable = false;
			this.after.ccA1LOD = JSON.parse(JSON.stringify(this.after.lod));
		}
	}

	cca2select() {
		if (this.after.ccA2Id > 0) {
			this.cca2disable = false;
			this.after.ccA2LOD = JSON.parse(JSON.stringify(this.after.lod));
		}
	}

	cca3select() {
		if (this.after.ccA3Id > 0) {
			this.cca3disable = false;
			this.after.ccA3LOD = JSON.parse(JSON.stringify(this.after.lod));
		}
	}

	//Check for duplicates selected
	checkDuplicates() {
		let duplicate: boolean = false;
		if (this.after.drI1Id > 0) {
			if (this.after.drI2Id > 0) {
				if (this.after.drI1Id === this.after.drI2Id) {
					this.errorMessage = "Duplicate driver 1 and 2 found!";
					this.failed = true;
					duplicate = true;
				}
			}
		}
		if (this.after.rN1Id > 0) {
			if (this.after.rN2Id > 0) {
				if (this.after.rN1Id === this.after.rN2Id) {
					this.errorMessage = "Duplicate RN 1 and 2 found!";
					this.failed = true;
					duplicate = true;
				}
			}
			if (this.after.rN3Id > 0) {
				if (this.after.rN1Id === this.after.rN3Id) {
					this.errorMessage = "Duplicate RN 1 and 3 found!";
					this.failed = true;
					duplicate = true;
				}
			}
		}
		if (this.after.rN2Id > 0) {
			if (this.after.rN2Id === this.after.rN3Id) {
				this.errorMessage = "Duplicate RN 2 and 3 found!";
				this.failed = true;
				duplicate = true;
			}
		}
		if (this.after.ccA1Id > 0) {
			if (this.after.ccA2Id > 0) {
				if (this.after.ccA1Id === this.after.ccA2Id) {
					this.errorMessage = "Duplicate CCA 1 and 2 found!";
					this.failed = true;
					duplicate = true;
				}
			}
			if (this.after.ccA3Id > 0) {
				if (this.after.ccA1Id === this.after.ccA3Id) {
					this.errorMessage = "Duplicate CCA 1 and 3 found!";
					this.failed = true;
					duplicate = true;
				}
			}
		}
		if (this.after.ccA2Id > 0) {
			if (this.after.ccA2Id === this.after.ccA3Id) {
				this.errorMessage = "Duplicate CCA 2 and 3 found!";
				this.failed = true;
				duplicate = true;
			}
		}
		return duplicate;
	}

	//Set name methods
	getNames() {
		try {
			this.after.sV1Name = this.$refs.sv1.$data.selectedItems[0].name;
		} catch{
			this.after.sV1Name = "";
		}
		try {
			this.after.drI1Name = this.$refs.dri1.$data.selectedItems[0].name;
		} catch{
			this.after.drI1Name = "";
		}
		try {
			this.after.drI2Name = this.$refs.dri2.$data.selectedItems[0].name;
		} catch{
			this.after.drI2Name = "";
		}
		try {
			this.after.rN1Name = this.$refs.rn1.$data.selectedItems[0].name;
		} catch{
			this.after.rN1Name = "";
		}
		try {
			this.after.rN2Name = this.$refs.rn2.$data.selectedItems[0].name;
		} catch{
			this.after.rN2Name = "";
		}
		try {
			this.after.rN3Name = this.$refs.rn3.$data.selectedItems[0].name;
		} catch{
			this.after.rN3Name = "";
		}
		try {
			this.after.ccA1Name = this.$refs.cca1.$data.selectedItems[0].name;
		} catch{
			this.after.ccA1Name = "";
		}
		try {
			this.after.ccA2Name = this.$refs.cca2.$data.selectedItems[0].name;
		} catch{
			this.after.ccA2Name = "";
		}
		try {
			this.after.ccA3Name = this.$refs.cca3.$data.selectedItems[0].name;
		} catch{
			this.after.ccA3Name = "";
		}
	}

	clear() {
		this.after.sV1Id = 0;
		this.after.sV1Name = "";
		this.after.sV1LOD = 0;
		this.after.sV1UNS = 0;
		this.after.sV1OT = 0;
		this.after.drI1Id = 0;
		this.after.drI1Name = "";
		this.after.drI1LOD = 0;
		this.after.drI1UNS = 0;
		this.after.drI1OT = 0;
		this.after.drI2Id = 0;
		this.after.drI2Name = "";
		this.after.drI2LOD = 0;
		this.after.drI2UNS = 0;
		this.after.drI2OT = 0;
		this.after.rN1Id = 0;
		this.after.rN1Name = "";
		this.after.rN1LOD = 0;
		this.after.rN1UNS = 0;
		this.after.rN1OT = 0;
		this.after.rN2Id = 0;
		this.after.rN2Name = "";
		this.after.rN2LOD = 0;
		this.after.rN2UNS = 0;
		this.after.rN2OT = 0;
		this.after.rN3Id = 0;
		this.after.rN3Name = "";
		this.after.rN3LOD = 0;
		this.after.rN3UNS = 0;
		this.after.rN3OT = 0;
		this.after.ccA1Id = 0;
		this.after.ccA1Name = "";
		this.after.ccA1LOD = 0;
		this.after.ccA1UNS = 0;
		this.after.ccA1OT = 0;
		this.after.ccA2Id = 0;
		this.after.ccA2Name = "";
		this.after.ccA2LOD = 0;
		this.after.ccA2UNS = 0;
		this.after.ccA2OT = 0;
		this.after.ccA3Id = 0;
		this.after.ccA3Name = "";
		this.after.ccA3LOD = 0;
		this.after.ccA3UNS = 0;
		this.after.ccA3OT = 0;
		this.sv1disable = true;
		this.dri1disable = true;
		this.dri2disable = true;
		this.rn1disable = true;
		this.rn2disable = true;
		this.rn3disable = true;
		this.cca1disable = true;
		this.cca2disable = true;
		this.cca3disable = true;
	}

	cancel() {
		this.$router.push('/fetchsession');
	}
}
