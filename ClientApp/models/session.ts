import { SessionEmployee } from "./sessionemployee";

export interface Session {
	id: number;
	date: string;
	day: string;
	type: string;
	site: string;
	time: string;
	lod: number;
	chairs: number;
	occ: number;
	estimate: number;
	holiday: number;
	note: string;
	staffCount: number;
	state: number;
	employees: SessionEmployee[];
}