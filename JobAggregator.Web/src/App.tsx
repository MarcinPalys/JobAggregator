import { useEffect, useState } from 'react';
import type { JobOffer } from './types/JobOffer';
import { jobService } from './services/jobService';
import type { PagedResult } from './services/jobService';
import { JobCard } from './components/JobCard';
import { Pagination } from './components/Pagination';

const detectLevel = (title: string): string => {
  const t = title.toLowerCase();
  if (t.includes('staż') || t.includes('stażyst') || t.includes('intern')) return 'Staż';
  if (t.includes('junior') || t.includes('jr.') || t.includes('jr ') || t.includes('młodszy')) return 'Junior';
  if (t.includes('mid') || t.includes('regular') || t.includes('specjalista')) return 'Mid';
  if (t.includes('senior') || t.includes('sr.') || t.includes('sr ') || t.includes('starszy')) return 'Senior';
  if (t.includes('lead') || t.includes('principal') || t.includes('architect')) return 'Lead';
  if (t.includes('manager') || t.includes('kierownik') || t.includes('dyrektor')) return 'Manager';
  return 'Nieokreślony';
};

function App() {
  const [pagedResult, setPagedResult] = useState<PagedResult | null>(null);
  const [jobs, setJobs] = useState<JobOffer[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [salaryFilter, setSalaryFilter] = useState('');
  const [sourceFilter, setSourceFilter] = useState('');
  const [cityFilter, setCityFilter] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('');
  const [levelFilter, setLevelFilter] = useState('');
  const PAGE_SIZE = 20;

 useEffect(() => {
  let cancelled = false;

  jobService.getPaged(page, PAGE_SIZE)
    .then(result => {
      if (!cancelled) {
        setPagedResult(result);
        setJobs(result.items);
        setLoading(false);
      }
    })
    .catch(() => {
      if (!cancelled) {
        setError('Nie udało się pobrać ofert pracy');
        setLoading(false);
      }
    });

  return () => {
    cancelled = true;
    setLoading(true);
  };
}, [page]);

  const cities = [...new Set(jobs.map(j => j.city).filter(Boolean))].sort();
  const categories = [...new Set(jobs.map(j => j.category).filter(Boolean))].sort();
  const sources = [...new Set(jobs.map(j => j.sourceName).filter(Boolean))].sort();

  const filtered = jobs.filter(j => {
    const matchSearch =
      j.title.toLowerCase().includes(search.toLowerCase()) ||
      j.company.toLowerCase().includes(search.toLowerCase()) ||
      j.location.toLowerCase().includes(search.toLowerCase());
    const matchCity = cityFilter === '' || j.city === cityFilter;
    const matchCategory = categoryFilter === '' || j.category === categoryFilter;
    const matchSalary =
      salaryFilter === '' ||
      (salaryFilter === 'with_salary' && j.salaryMin != null) ||
      (salaryFilter === 'no_salary' && j.salaryMin == null);
    const matchSource = sourceFilter === '' || j.sourceName === sourceFilter;
    const matchLevel = levelFilter === '' || detectLevel(j.title) === levelFilter;
    return matchSearch && matchCity && matchCategory && matchSalary && matchSource && matchLevel;
  });

  const clearFilters = () => {
    setSearch('');
    setCityFilter('');
    setCategoryFilter('');
    setSalaryFilter('');
    setSourceFilter('');
    setLevelFilter('');
  };

  const hasFilters = search || cityFilter || categoryFilter || salaryFilter || sourceFilter || levelFilter;

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-gradient-to-r from-blue-600 to-blue-800 shadow-lg sticky top-0 z-10">
        <div className="max-w-5xl mx-auto px-4 py-4">
          <div className="flex items-center gap-4">
            <div className="flex items-center gap-2">
              <div className="bg-white rounded-lg p-2">
                <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                </svg>
              </div>
              <div>
                <h1 className="text-xl font-bold text-white">Job Aggregator</h1>
                <p className="text-blue-200 text-xs">Znajdź wymarzoną pracę</p>
              </div>
            </div>
            <input
              type="text"
              placeholder="Szukaj po tytule, firmie lub lokalizacji..."
              value={search}
              onChange={e => setSearch(e.target.value)}
              className="flex-1 border-0 rounded-lg px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-300 bg-white/20 text-white placeholder-blue-200"
            />
          </div>
        </div>
      </header>

      <div className="bg-white border-b border-gray-200 shadow-sm">
        <div className="max-w-5xl mx-auto px-4 py-3 flex flex-wrap items-center gap-3">
          <select value={cityFilter} onChange={e => setCityFilter(e.target.value)}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-gray-700">
            <option value="">Wszystkie miasta</option>
            {cities.map(city => <option key={city} value={city}>{city}</option>)}
          </select>

          <select value={categoryFilter} onChange={e => setCategoryFilter(e.target.value)}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-gray-700">
            <option value="">Wszystkie kategorie</option>
            {categories.map(cat => <option key={cat} value={cat}>{cat}</option>)}
          </select>

          <select value={levelFilter} onChange={e => setLevelFilter(e.target.value)}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-gray-700">
            <option value="">Wszystkie poziomy</option>
            <option value="Staż">Staż</option>
            <option value="Junior">Junior</option>
            <option value="Mid">Mid</option>
            <option value="Senior">Senior</option>
            <option value="Lead">Lead</option>
            <option value="Manager">Manager</option>
            <option value="Nieokreślony">Nieokreślony</option>
          </select>

          <select value={salaryFilter} onChange={e => setSalaryFilter(e.target.value)}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-gray-700">
            <option value="">Wszystkie oferty</option>
            <option value="with_salary">Z podanym wynagrodzeniem</option>
            <option value="no_salary">Bez wynagrodzenia</option>
          </select>

          <select value={sourceFilter} onChange={e => setSourceFilter(e.target.value)}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-gray-700">
            <option value="">Wszystkie źródła</option>
            {sources.map(src => <option key={src} value={src}>{src}</option>)}
          </select>

          {hasFilters && (
            <button onClick={clearFilters}
              className="text-sm text-red-500 hover:text-red-700 px-3 py-2 rounded-lg hover:bg-red-50 transition-colors">
              Wyczyść filtry
            </button>
          )}

          <span className="ml-auto text-sm text-gray-500">
            Znaleziono <span className="font-semibold text-gray-800">
              {pagedResult?.totalCount ?? 0}
            </span> ofert
          </span>
        </div>
      </div>

      <main className="max-w-5xl mx-auto px-4 py-6">
        {loading && <div className="text-center py-20 text-gray-500">Ładowanie ofert...</div>}
        {error && <div className="text-center py-20 text-red-500">{error}</div>}
        {!loading && !error && filtered.length === 0 && (
          <div className="text-center py-20 text-gray-500">Brak ofert spełniających kryteria</div>
        )}
        {!loading && !error && (
          <div className="flex flex-col gap-4">
            {filtered.map(job => <JobCard key={job.id} job={job} />)}
          </div>
        )}
        {pagedResult && (
          <Pagination
            page={pagedResult.page}
            totalPages={pagedResult.totalPages}
            hasNextPage={pagedResult.hasNextPage}
            hasPreviousPage={pagedResult.hasPreviousPage}
            onPageChange={handlePageChange}
          />
        )}
      </main>
    </div>
  );
}

export default App;