import type { JobOffer } from '../types/JobOffer';

interface Props {
  job: JobOffer;
}

export const JobCard = ({ job }: Props) => {
  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 hover:shadow-md transition-shadow">
      <div className="flex justify-between items-start gap-4">
        <div className="flex-1">
          <h2 className="text-lg font-semibold text-gray-900">{job.title}</h2>
          <p className="text-gray-600 mt-1">{job.company}</p>
          <div className="flex flex-wrap gap-2 mt-3">
            {job.location && (
              <span className="text-sm bg-gray-100 text-gray-600 px-3 py-1 rounded-full">
                {job.location}
              </span>
            )}
            {job.salaryMin && job.salaryMax && (
              <span className="text-sm bg-green-100 text-green-700 px-3 py-1 rounded-full">
                {job.salaryMin.toLocaleString()} – {job.salaryMax.toLocaleString()} {job.currency}
              </span>
            )}
            <span className="text-sm bg-blue-100 text-blue-700 px-3 py-1 rounded-full">
              {job.sourceName}
            </span>
          </div>
        </div>
        
         <a href={job.sourceUrl}
          target="_blank"
          rel="noopener noreferrer"
          className="shrink-0 bg-blue-600 text-white text-sm px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          Zobacz ofertę
        </a>
      </div>
    </div>
  );
};