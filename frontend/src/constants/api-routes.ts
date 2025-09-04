const ROOT = "api"
const VERSION = "v1"

const apiBase = (...parts: string[]) => [ROOT, VERSION, ...parts].join("/")

const makeAuthRoutes = (base = apiBase("auth")) => ({
	post() {
		return {
			login: `${base}/login`,
			register: `${base}/register`,
			logout: `${base}/logout`,
		}
	},

	get() {
		return {
			currentUser: `${base}/me`,
		}
	},
})

const makeExpenseRoutes = (base = apiBase("expense")) => ({
	post() {
		return {
			create: base,
		}
	},

	get() {
		return {
			byId: `${base}/{id}`,
			all: `${base}/getall`,
			filter: `${base}/filter`,
			total: `${base}/total`,
		}
	},

	put() {
		return {
			update: `${base}/{id}`,
		}
	},

	delete() {
		return {
			byId: `${base}/{id}`,
			all: `${base}/all`,
		}
	},
})

const makeBudgetRoutes = (base = apiBase("budget")) => ({
	post() {
		return {
			create: base,
		}
	},

	get() {
		return {
			status: `${base}/status`,
			overview: `${base}/overview`,
		}
	},

	put() {
		return {
			update: `${base}/update/{id}`,
		}
	},

	delete() {
		return {
			remove: `${base}/delete/{id}`,
		}
	},
})

const makeSavingsRoutes = (base = apiBase("savings")) => ({
	post() {
		return {
			create: base,
			contribute: `${base}/contribute`,
		}
	},

	get() {
		return {
			all: `${base}/getall`,
			byId: `${base}/{id}`,
		}
	},

	put() {
		return {
			update: `${base}/{id}`,
		}
	},

	patch() {
		return {
			archive: `${base}/{id}/archive`,
		}
	},

	delete() {
		return {
			byId: `${base}/{id}`,
		}
	},
})

const makeDashboardRoutes = (base = apiBase("dashboard")) => ({
	get() {
		return {
			summary: `${base}/summary`,
		}
	},
})

const makeMetadataRoutes = (base = apiBase("metadata")) => ({
	get() {
		return {
			expenseCategories: `${base}/expense-categories`,
			paymentMethods: `${base}/payment-methods`,
			savingGoalStatuses: `${base}/saving-goal-statuses`,
		}
	},
})

export const ApiRoutes = {
	auth: makeAuthRoutes(),
	expense: makeExpenseRoutes(),
	budget: makeBudgetRoutes(),
	savings: makeSavingsRoutes(),
	dashboard: makeDashboardRoutes(),
	metadata: makeMetadataRoutes(),
} as const
