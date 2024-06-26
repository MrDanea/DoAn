﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using System.Text;
using System.Threading.Tasks;
using DoAn.Models;
using DoAn.Services;

namespace DoAn.ViewModels
{
    public class StationDetailViewModel : ObservableObject
    {
        public string _stationId;
        private string _stationName;
        private string _stationAddress;
        public string StationName { get => _stationName; set => SetProperty(ref _stationName, value); }
        public string StationAddress { get => _stationAddress; set => SetProperty(ref _stationAddress, value); }
        public string StationId 
        { 
            get => _stationId; 
            set => SetProperty(ref _stationId,value); 
        }
        public Document DataResponse { get; set; }
        public ObservableCollection<StationDetailModel> _windchart;
        public ObservableCollection<StationDetailModel> _seachart;
        private ObservableCollection<StationDetailModel> _winddataGrid;
        private ObservableCollection<StationDetailModel> _seadataGrid;

        public ObservableCollection<StationDetailModel> WindChart
        {
            get { return _windchart; }
            set { this._windchart = value; OnPropertyChanged(); }
        }
        public ObservableCollection<StationDetailModel> SeaChart
        {
            get { return _seachart; }
            set { this._seachart = value; OnPropertyChanged(); }
        }
        public ObservableCollection<StationDetailModel> WindDataGrid
        {
            get { return _winddataGrid; }
            set { this._winddataGrid = value; OnPropertyChanged(); }
        }
        public ObservableCollection<StationDetailModel> SeaDataGrid
        {
            get { return _seadataGrid; }
            set { this._seadataGrid = value; OnPropertyChanged(); }
        }

        public event Action ResponseHandle;
        protected virtual void OnResponseHandleEvent()
        {
            ResponseHandle?.Invoke();
        }
        public StationDetailViewModel(string station) 
        {
            StationId = station;
            ResponseHandle += OnResponseHandle;
            WindDataGrid = [];
            WindChart = [];
            SeaChart = [];
            SeaDataGrid = []; 
            SendandListen();
        }
        public async void SendandListen()
        {
            MQTT.Broker.Instance.Send($"dane/service/stationdetail/{Service.Instance.UserID}", new Document() { Token = $"{Service.Instance.Token}", StationID = $"{this.StationId}" });
            await Task.Delay(50);
            MQTT.Broker.Instance.Listen($"dane/service/stationdetail/{Service.Instance.UserID}", HandleReceivedData);
            
        }
        private void HandleReceivedData(Document doc)
        {
            if (doc != null)
            {
                this.DataResponse = doc;
                this.StationName = this.DataResponse.StationName;
                this.StationAddress = this.DataResponse.StationAddress;
                OnResponseHandleEvent();
            }
        }
        private void OnResponseHandle()
        {
            List<string> dl = DataResponse.StationTypeList;
            Func<string, int> convert = (input) => {
                if (dl.Contains($"{input}")) { return 1; }
                else return 0;
            };
            if (dl != null)
            {
                int act = (convert("Hydrological") << 2) | (convert("Meteorological") << 1) | convert("RainFall");
                switch (act)
                {
                    case 7: HydroData(DataResponse); MeteoData(DataResponse); break;  // 111
                    case 6: HydroData(DataResponse); MeteoData(DataResponse); break; //110
                    case 5: MeteoData(DataResponse); break; //101
                    case 4: MeteoData(DataResponse); break; //100
                    case 3: MeteoData(DataResponse); break; //011
                    case 2: MeteoData(DataResponse); break; //010
                    case 1: Console.WriteLine(); break; //001
                    case 0: Console.WriteLine("Ehe"); break; //000
                    default: break;
                }
            }
        } 
        private void MeteoData(Document doc)
        {
            ObservableCollection<StationDetailModel> datagrid = new ObservableCollection<StationDetailModel>();
            ObservableCollection<StationDetailModel> chart = new ObservableCollection<StationDetailModel>();

            foreach (var item in doc.StationData)
            {
                datagrid.Add(
                    new StationDetailModel(
                    item.ObjectId,
                    item.WindSpeed,
                    item.WindDirection,
                    item.WindSpeedAt2mHeight,
                    item.WindDirectionAt2mHeight,
                    item.TimeOfOccurrenceOffxfx2m,
                    item.AverageWindSpeedIn2s,
                    item.AverageWindDirectionIn2s,
                    item.TimeOfOccurrenceOffxfx2s)
                    );
                double yvalue = ConvertStringToDouble(item.WindSpeedAt2mHeight);
                chart.Add(new StationDetailModel(item.ObjectId, yvalue));
            }
            this.WindDataGrid = datagrid;
            this.WindChart = chart;
        }
        private void HydroData(Document doc)
        {
            ObservableCollection<StationDetailModel> datagrid = new ObservableCollection<StationDetailModel>();
            ObservableCollection<StationDetailModel> chart = new ObservableCollection<StationDetailModel>();

            foreach (var item in doc.StationData)
            {
                datagrid.Add(
                    new StationDetailModel(
                    item.ObjectId,
                    item.SeaLevel,
                    item.WaveHeight,
                    item.WaveLength,
                    item.WaveHeightMax)
                    );
                double yvalue = ConvertStringToDouble(item.SeaLevel);
                chart.Add(new StationDetailModel(item.ObjectId, yvalue));
            }
            this.SeaDataGrid = datagrid;
            this.SeaChart = chart;
        }
        public static double ConvertStringToDouble(string input)
        {
            if (double.TryParse(input, out double result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"The input string '{input}' is not in a correct format to be converted to a double.");
            }
        }

    }
}
