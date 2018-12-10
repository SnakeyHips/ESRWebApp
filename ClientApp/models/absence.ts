export interface Absence {
	id: number;
	employeeId: number;
	employeeName: string;
	type: string;
	startDate: string;
	endDate: string;
	partDay: string;
	hours: number;
}