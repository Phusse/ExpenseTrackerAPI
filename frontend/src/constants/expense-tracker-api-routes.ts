const ROOT = "api";
const VERSION = "v1";

const apiBase = (...parts: string[]) => [ROOT, VERSION, ...parts].join("/");

const makeAuthRoutes = (base = apiBase("auth")) => ({
	post: {
		login: `${base}/login`,
		register: `${base}/register`,
		logout: `${base}/logout`,
	},
	get: {
		currentUser: `${base}/me`,
	},
});

const makeExpenseRoutes = (base = apiBase("expense")) => ({
	post: {
		create: base,
	},
	get: {
		byId: `${base}/{id}`,
		all: `${base}/getall`,
		filter: `${base}/filter`,
		total: `${base}/total`,
	},
	put: {
		update: `${base}/{id}`,
	},
	delete: {
		byId: `${base}/{id}`,
		all: `${base}/all`,
	},
});

const makeBudgetRoutes = (base = apiBase("budget")) => ({
	post: {
		create: base,
	},
	get: {
		status: `${base}/status`,
		overview: `${base}/overview`,
	},
	put: {
		update: `${base}/update/{id}`,
	},
	delete: {
		remove: `${base}/delete/{id}`,
	},
});

const makeSavingsRoutes = (base = apiBase("savings")) => ({
	post: {
		create: base,
		contribute: `${base}/contribute`,
	},
	get: {
		all: `${base}/getall`,
		byId: `${base}/{id}`,
	},
	put: {
		update: `${base}/{id}`,
	},
	patch: {
		archive: `${base}/{id}/archive`,
	},
	delete: {
		byId: `${base}/{id}`,
	},
});

const makeDashboardRoutes = (base = apiBase("dashboard")) => ({
	get: {
		summary: `${base}/summary`,
	},
});

const makeMetadataRoutes = (base = apiBase("metadata")) => ({
	get: {
		expenseCategories: `${base}/expense-categories`,
		paymentMethods: `${base}/payment-methods`,
		savingGoalStatuses: `${base}/saving-goal-statuses`,
	},
});

export const ExpenseTrackerApiRoutes = {
	auth: makeAuthRoutes(),
	expense: makeExpenseRoutes(),
	budget: makeBudgetRoutes(),
	savings: makeSavingsRoutes(),
	dashboard: makeDashboardRoutes(),
	metadata: makeMetadataRoutes(),
} as const;
