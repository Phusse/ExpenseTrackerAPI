"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import type { ExpenseFilters } from "@/types/expense"
import { Filter, X } from "lucide-react"

interface ExpenseFiltersProps {
  onFilter: (filters: ExpenseFilters) => void
  onClear: () => void
}

const categories = [
  "Food & Dining",
  "Transportation",
  "Shopping",
  "Entertainment",
  "Bills & Utilities",
  "Healthcare",
  "Travel",
  "Education",
  "Business",
  "Other",
]

export function ExpenseFiltersComponent({ onFilter, onClear }: ExpenseFiltersProps) {
  const [filters, setFilters] = useState<ExpenseFilters>({})
  const [isExpanded, setIsExpanded] = useState(false)

  const handleFilter = () => {
    onFilter(filters)
  }

  const handleClear = () => {
    setFilters({})
    onClear()
  }

  const hasActiveFilters = Object.values(filters).some((value) => value !== undefined && value !== "" && value !== null)

  return (
    <Card>
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between">
          <CardTitle className="flex items-center gap-2">
            <Filter className="w-4 h-4" />
            Filters
          </CardTitle>
          <Button variant="ghost" size="sm" onClick={() => setIsExpanded(!isExpanded)}>
            {isExpanded ? "Hide" : "Show"}
          </Button>
        </div>
      </CardHeader>

      {isExpanded && (
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="startDate">Start Date</Label>
              <Input
                id="startDate"
                type="date"
                value={filters.startDate || ""}
                onChange={(e) => setFilters((prev) => ({ ...prev, startDate: e.target.value }))}
              />
            </div>

            <div>
              <Label htmlFor="endDate">End Date</Label>
              <Input
                id="endDate"
                type="date"
                value={filters.endDate || ""}
                onChange={(e) => setFilters((prev) => ({ ...prev, endDate: e.target.value }))}
              />
            </div>
          </div>

          <div>
            <Label htmlFor="category">Category</Label>
            <Select
              value={filters.category || ""}
              onValueChange={(value) => setFilters((prev) => ({ ...prev, category: value || undefined }))}
            >
              <SelectTrigger>
                <SelectValue placeholder="All categories" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All categories</SelectItem>
                {categories.map((category) => (
                  <SelectItem key={category} value={category}>
                    {category}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <Label htmlFor="minAmount">Min Amount</Label>
              <Input
                id="minAmount"
                type="number"
                step="0.01"
                placeholder="0.00"
                value={filters.minAmount?.toString() || ""}
                onChange={(e) =>
                  setFilters((prev) => ({
                    ...prev,
                    minAmount: e.target.value ? Number.parseFloat(e.target.value) : undefined,
                  }))
                }
              />
            </div>

            <div>
              <Label htmlFor="maxAmount">Max Amount</Label>
              <Input
                id="maxAmount"
                type="number"
                step="0.01"
                placeholder="0.00"
                value={filters.maxAmount?.toString() || ""}
                onChange={(e) =>
                  setFilters((prev) => ({
                    ...prev,
                    maxAmount: e.target.value ? Number.parseFloat(e.target.value) : undefined,
                  }))
                }
              />
            </div>

            <div>
              <Label htmlFor="exactAmount">Exact Amount</Label>
              <Input
                id="exactAmount"
                type="number"
                step="0.01"
                placeholder="0.00"
                value={filters.exactAmount?.toString() || ""}
                onChange={(e) =>
                  setFilters((prev) => ({
                    ...prev,
                    exactAmount: e.target.value ? Number.parseFloat(e.target.value) : undefined,
                  }))
                }
              />
            </div>
          </div>

          <div className="flex gap-2">
            <Button onClick={handleFilter} className="flex-1">
              Apply Filters
            </Button>
            {hasActiveFilters && (
              <Button variant="outline" onClick={handleClear}>
                <X className="w-4 h-4 mr-2" />
                Clear
              </Button>
            )}
          </div>
        </CardContent>
      )}
    </Card>
  )
}
