"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { ExpenseAPI } from "@/lib/expense-api"
import type { TotalExpenseParams } from "@/types/expense"
import { DollarSign, TrendingUp, Calendar, BarChart3 } from "lucide-react"

export function ExpenseDashboard() {
  const [totalExpense, setTotalExpense] = useState<number>(0)
  const [totalMessage, setTotalMessage] = useState<string>("")
  const [isLoading, setIsLoading] = useState(false)
  const [filters, setFilters] = useState<TotalExpenseParams>({})

  const currentYear = new Date().getFullYear()
  const currentMonth = new Date().getMonth() + 1

  const months = [
    { value: 1, label: "January" },
    { value: 2, label: "February" },
    { value: 3, label: "March" },
    { value: 4, label: "April" },
    { value: 5, label: "May" },
    { value: 6, label: "June" },
    { value: 7, label: "July" },
    { value: 8, label: "August" },
    { value: 9, label: "September" },
    { value: 10, label: "October" },
    { value: 11, label: "November" },
    { value: 12, label: "December" },
  ]

  const years = Array.from({ length: 10 }, (_, i) => currentYear - i)

  const fetchTotal = async (params: TotalExpenseParams = {}) => {
    setIsLoading(true)
    try {
      const response = await ExpenseAPI.getTotalExpense(params)
      if (response.success) {
        // Extract total from message - this is a bit hacky but matches your API
        const match = response.message.match(/[\d,]+\.?\d*/)
        const total = match ? Number.parseFloat(match[0].replace(",", "")) : 0
        setTotalExpense(total)
        setTotalMessage(response.message)
      }
    } catch (error) {
      console.error("Failed to fetch total:", error)
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    // Load current month total by default
    fetchTotal({ month: currentMonth, year: currentYear })
  }, [])

  const handleFilterChange = (key: keyof TotalExpenseParams, value: string | number | undefined) => {
    const newFilters = { ...filters, [key]: value }
    setFilters(newFilters)
    fetchTotal(newFilters)
  }

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(amount)
  }

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Expenses</CardTitle>
            <DollarSign className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{isLoading ? "Loading..." : formatCurrency(totalExpense)}</div>
            <p className="text-xs text-muted-foreground mt-1">{totalMessage}</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">This Month</CardTitle>
            <Calendar className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <Button
              variant="outline"
              size="sm"
              onClick={() => {
                const params = { month: currentMonth, year: currentYear }
                setFilters(params)
                fetchTotal(params)
              }}
            >
              View Current
            </Button>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">This Year</CardTitle>
            <TrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <Button
              variant="outline"
              size="sm"
              onClick={() => {
                const params = { year: currentYear }
                setFilters(params)
                fetchTotal(params)
              }}
            >
              View {currentYear}
            </Button>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">All Time</CardTitle>
            <BarChart3 className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <Button
              variant="outline"
              size="sm"
              onClick={() => {
                setFilters({})
                fetchTotal({})
              }}
            >
              View All
            </Button>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Custom Total Calculation</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div>
              <label className="text-sm font-medium">Month</label>
              <Select
                value={filters.month?.toString() || ""}
                onValueChange={(value) => handleFilterChange("month", value ? Number.parseInt(value) : undefined)}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select month" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All months</SelectItem>
                  {months.map((month) => (
                    <SelectItem key={month.value} value={month.value.toString()}>
                      {month.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <label className="text-sm font-medium">Year</label>
              <Select
                value={filters.year?.toString() || ""}
                onValueChange={(value) => handleFilterChange("year", value ? Number.parseInt(value) : undefined)}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select year" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All years</SelectItem>
                  {years.map((year) => (
                    <SelectItem key={year} value={year.toString()}>
                      {year}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <label className="text-sm font-medium">Start Date</label>
              <input
                type="date"
                className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                value={filters.startDate || ""}
                onChange={(e) => handleFilterChange("startDate", e.target.value)}
              />
            </div>

            <div>
              <label className="text-sm font-medium">End Date</label>
              <input
                type="date"
                className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                value={filters.endDate || ""}
                onChange={(e) => handleFilterChange("endDate", e.target.value)}
              />
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
