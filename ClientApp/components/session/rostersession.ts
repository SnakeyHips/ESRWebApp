import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Session } from '../../models/session';
import { Employee } from '../../models/employee';

@Component
export default class RosterSessionComponent extends Vue {

	mount: boolean = false;
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
	
	employees: Employee[] = [];
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
				//then get available 
				this.getAvailable();
			});
	}

	getAvailable() {
		fetch('api/Employee/GetAvailable?date=' + this.before.date)
			.then(response => response.json() as Promise<Employee[]>)
			.then(data => {
				this.employees = data;
				this.filterRoles();
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
		this.mount = true;
	}



	clearSV1() {
		console.log(this.after.sV1Id);
		console.log(this.after.sV1Name);
		//var select = (document.getElementById("roster-sv1Id")) as HTMLSelectElement;
		//select.selectedIndex = -1;
	}

	rosterSession() {
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
					alert("Failed to create update Roster");
				} else {
					this.$router.push('/fetchsession');
				}
			})
	}

	//Set name methods
	setSV1(name: string) {
		this.after.sV1Name = name;
	}

	setDRI1(name: string) {
		this.after.drI1Name = name;
	}

	setDRI2(name: string) {
		this.after.drI2Name = name;
	}

	setRN1(name: string) {
		this.after.rN1Name = name;
	}

	setRN2(name: string) {
		this.after.rN2Name = name;
	}

	setRN3(name: string) {
		this.after.rN3Name = name;
	}

	setCCA1(name: string) {
		this.after.ccA1Name = name;
	}

	setCCA2(name: string) {
		this.after.ccA2Name = name;
	}

	setCCA3(name: string) {
		this.after.ccA3Name = name;
	}
}
