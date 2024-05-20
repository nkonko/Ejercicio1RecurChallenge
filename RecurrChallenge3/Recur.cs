namespace Test;

public class SaveAvailabilityCommand
{
    private readonly IReceiveAvailabilityProcessor receiveAvailabilityProcessor;
    private readonly IUnitOfWork unitOfWork;

    public SaveAvailabilityCommand(IReceiveAvailabilityProcessor receiveAvailabilityProcessor, IUnitOfWork unitOfWork)
    {
        this.receiveAvailabilityProcessor = receiveAvailabilityProcessor;
        this.unitOfWork = unitOfWork;
    }

    public void SaveReceiveAvailability(ReceiveAvailabilityDTO receiveAvailabilityDto)
    {
        var isValidDTO = validateReceiveAvailabilityDto(receiveAvailabilityDto);

        if (isValidDTO)
        {
            var imputationList = receiveAvailabilityDto.listDateImputation;

            foreach (DateImputationDTO dateImputation in imputationList)
            {
                receiveAvailabilityProcessor.Process(dateImputation, receiveAvailabilityDto);
            }

            this.unitOfWork.Commit();
        }
    }
}

public class ReceiveAvailabilityProcessor : IReceiveAvailabilityProcessor
{
    private readonly IDateFillProcessor dateFillProcessor;
    private readonly ICreateReceiveAvailabilitiesCommand createReceiveAvailabilitiesCommand

    public ReceiveAvailabilityProcessor(IDateFillProcessor dateFillProcessor, ICreateReceiveAvailabilitiesCommand createReceiveAvailabilitiesCommand)
    {
        this.dateFillProcessor = dateFillProcessor;
        this.createReceiveAvailabilitiesCommand = createReceiveAvailabilitiesCommand;
    }

    public void Process(DateImputationDTO dateImputation, ReceiveAvailabilityDTO receiveAvailabilityDto)
    {
        var searchParams = BuildReceiveAvailability();
        var searchParamsEmployeeDc = BuildsearchParamsEmployeeDc();
        var searchParamsCompany = BuildCompany();
        var searchParamsEmployeePersonal = new BuildEmployeePersonal();

        if (CheckTimeListAnyNotNull(dateImputation))
        {
            dateFillProcessor.FillDatesDto(dateImputation);
        }

        var employeeDc = _employeeDcRepository.GetEmployeeDcActive(searchParamsEmployeeDc);
        var company = _companyRepository.GetCompany(searchParamsCompany);
        var employeePersonal = _employeePersonalDataRepository.GetEmployee(searchParamsEmployeePersonal);

        validateReceiveAvailability(receiveAvailabilityDto, employeeDc, company, employeePersonal, 2, dateImputation);

        searchParams.IdEmployeeDc = employeeDc.IdEmployeeDC;
        var listReceiveAvailabilitiesFiltrated = _receiveAvailabilityRepository
                                                                      .GetReceiveAvailability(searchParams)
                                                                      .Where(x => x.DateImputation >= dateImputation.DateImputation);

        foreach (var availabilityRemove in listReceiveAvailabilitiesFiltrated)
        {
            _receiveAvailabilityRepository.Delete(availabilityRemove);
        }

        if (CheckTimeListAnyNotNull(dateImputation))
        {
            createReceiveAvailabilitiesCommand.CreateReceiveAvailabilities(dateImputation);
        }
    }

    private bool CheckTimeListAnyNotNull(DateImputationDTO dateImputation)
    {
        return dateImputation.listAvailableTimes != null && dateImputation.listAvailableTimes.Any();
    }

    private ReceiveAvailability BuildReceiveAvailability()
    {
        return new ReceiveAvailability()
        {
            IdEmployeeDc = string.Empty,
            DateImputation = dateImputation.DateImputation
        }
}

    private EmployeeDc BuildEmployeeDc()
    {
        return new EmployeeDc()
        {
            IsActive = true,
            StartDateDC = dateImputation.DateImputation,
            IdEmployee = $"{receiveAvailabilityDto.IdCountry}·{receiveAvailabilityDto.CodEmployee}",
            IdCountry = receiveAvailabilityDto.IdCountry
        }
    }

    private Company BuildCompany()
    {
        return new Company()
        {
            IdCompany = $"{receiveAvailabilityDto.IdCountry}·{receiveAvailabilityDto.IdCompany},
            IdCountry = receiveAvailabilityDto.IdCountry
        }
    }

    private EmployeePersonal BuildEmployeePersonal()
    {
        return new EmployeePersonal()
        {
            IdEmployee = $"{receiveAvailabilityDto.IdCountry}·{receiveAvailabilityDto.CodEmployee}"
        }
    }
}

public class DateFillProcessor : IDateFillProcessor
{
    public void FillDatesDto(DateImputationDTO dateImputation)
    {
        for (int i = 0; i < dateImputation.listAvailableTimes.Count(); i++)
        {
            var timeReceive = dateImputation.listAvailableTimes[i];

            var hasToAddDay = i == dateImputation.listAvailableTimes.Count() - 1 &&
                timeReceive.EndDate.GetValueOrDefault() < timeReceive.StartDate.GetValueOrDefault();

            timeReceive.StartDate = FillDate(timeReceive.StartTime, dateImputation);

            timeReceive.EndDate = FillDate(timeReceive.EndDate, dateImputation, hasToAddDay);
        }
    }

    private DateTime FillDate(DateTime timeReceive, DateImputationDTO dateImputation, bool hasToAddDay = false)
    {
        if (timeReceive != null)
        {
            return timeReceive;
        }

        return hasToAddDay ? timeReceive.GetValueOrDefault().AddDays(1) :
                                new DateTime(dateImputation.DateImputation.Year,
                                           dateImputation.DateImputation.Month,
                                           dateImputation.DateImputation.Day,
                                           Convert.ToInt32(timeReceive.Substring(0, 2)),
                                           Convert.ToInt32(timeReceive.Substring(3, 2)),
                                           0);
    }
}

public class CreateReceiveAvailabilitiesCommand : ICreateReceiveAvailabilitiesCommand
{
    private readonly IRepository<ReceiveAvailability> _receiveAvailabilityRepository;

    public CreateReceiveAvailabilitiesCommand(IRepository<ReceiveAvailability> receiveAvailabilityRepository)
    {
        this._receiveAvailabilityRepository = receiveAvailabilityRepository;
    }

    public void CreateReceiveAvailabilities(DateImputationDTO dto)
    {
        foreach (var availabilityTimes in dto.listAvailableTimes)
        {
            var availabilityInsert = new ReceiveAvailability
            {
                IdEmployeeAvailability = Guid.NewGuid().ToString(),
                IdEmployeeDc = employeeDc.IdEmployeeDC,
                DateImputation = dto.DateImputation,
                IdCountry = receiveAvailabilityDto.IdCountry,
                StartTime = availabilityTimes.StartDate.GetValueOrDefault(),
                EndTime = availabilityTimes.EndDate.GetValueOrDefault(),
                CreationDate = DateTime.UtcNow.Date,
                CreationDescription = "IntegrationAPP-RRHH"
            };

            _receiveAvailabilityRepository.Insert(availabilityInsert);
        }
    }
}
