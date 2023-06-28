using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service {
    internal sealed class GameService : IGameService {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly GameConfiguration _gameConfiguration;

        
        public GameService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IConfiguration configuration) {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _gameConfiguration = new GameConfiguration();
            configuration.Bind(_gameConfiguration.Section, _gameConfiguration);
        }


        public async Task<GameDto> CreateNewGameAsync(bool trackChanges) {
            byte[] problem, solution;

            if (_gameConfiguration.UseTestData) {
                CrateTestProblemAndSolution(out problem, out solution);
            }
            else
            {
                if (System.OperatingSystem.IsWindows()) {
                    CreateGameExternalSharedMem(out problem, out solution);
                }
                else {
                    CreateGameExternalFile(out problem, out solution);
                }
            }

            GameDto gameDto = new GameDto() {
                id = Guid.NewGuid(),
                Solution = Convert.ToBase64String(solution),
                Problem = Convert.ToBase64String(problem),
                CreatedAt = DateTime.UtcNow
            };

            var game = _mapper.Map<Game>(gameDto);
            await _repository.Game.CreateNewGameAsync(game, trackChanges);
            await _repository.SaveAsync();
            return gameDto;
        }


        public async Task<GameDto> GetByIdAsnyc(Guid gameId, bool trackChanges) {
            var game = await _repository.Game.GetGameByIdAsync(gameId, trackChanges);
            if (game == null) {
                throw new GameNotFoundException(gameId);
            }

            var gameDto = _mapper.Map<GameDto>(game);
            return gameDto;
        }


        private string GetRandomName(int size) {
            string chars = "abcdefghijklmnopqrstuvwxyz";
            var rnd = new Random();

            var name = new string(
                Enumerable.Repeat(chars, size).Select(s => s[rnd.Next(chars.Length)]).ToArray()
            );

            return name;
        }


        private void CreateGameExternalSharedMem(out byte[] problem, out byte[] solution) {
            byte[] totalData = new byte[(int)_gameConfiguration.GameSize*2];
            problem = new byte[(int)_gameConfiguration.GameSize];
            solution = new byte[(int)_gameConfiguration.GameSize];
            int bytesRead = 0;

            var name = GetRandomName(8);

            using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew(
                    "sudoku_mm_"+name, _gameConfiguration.GameSize*2, MemoryMappedFileAccess.ReadWriteExecute)) {

                //Semaphore sem = new Semaphore(0, 1, "sudoku_sem_"+name);
                //There is no need for sync as pProcess.WaitForExit() is used
                CallExternalApplication(name);

                /*
                if (!sem.WaitOne(10000)) {
                    throw new Exception("External app didn't respond... ");
                }
                */
                using (MemoryMappedViewStream stream = mmf.CreateViewStream()) {
                    BinaryReader reader = new BinaryReader(stream);
                    bytesRead = reader.Read(totalData);
                }
            }

            if (bytesRead != _gameConfiguration.GameSize * 2) {
                throw new Exception("External app returned wrong data.");
            }

            for (int i = 0; i < _gameConfiguration.GameSize; i++) {
                problem[i] = totalData[i];
                solution[i] = totalData[i + _gameConfiguration.GameSize];
            }
        }


        private void CreateGameExternalFile(out byte[] problem, out byte[] solution) {
            byte[] totalData = new byte[(int)_gameConfiguration.GameSize * 2];
            problem = new byte[(int)_gameConfiguration.GameSize];
            solution = new byte[(int)_gameConfiguration.GameSize];
            int bytesRead = 0;

            var name = Path.GetTempPath() + GetRandomName(8);

            CallExternalApplication(name);

            using (FileStream fs = File.OpenRead(name)) {
                using (BinaryReader reader = new BinaryReader(fs)) {
                    bytesRead = reader.Read(totalData, 0, (int)(_gameConfiguration.GameSize * 2));
                }
            }
            File.Delete(name);

            //Semaphore sem = new Semaphore(0, 1, "sudoku_sem_"+name);
            //There is no need for sync as pProcess.WaitForExit() is used
            //CallExternalApplication(name);

            if (bytesRead != _gameConfiguration.GameSize * 2) {
                throw new Exception("External app returned wrong data.");
            }

            for (int i = 0; i < _gameConfiguration.GameSize; i++) {
                problem[i] = totalData[i];
                solution[i] = totalData[i + _gameConfiguration.GameSize];
            }
        }


        private void CallExternalApplication(string sysObjectsName) {
            string output = "";
            
            using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process()) {
                pProcess.StartInfo.FileName = _gameConfiguration.ExternalAppPath;
                pProcess.StartInfo.Arguments = sysObjectsName;
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = false;
                pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                pProcess.StartInfo.CreateNoWindow = true;
                pProcess.Start();
                pProcess.WaitForExit();
                output = pProcess.ExitCode.ToString();
            }

            if (output != "0") {
                throw new Exception($"External app returned: {output}");
            }
        }
    

        private void CrateTestProblemAndSolution(out byte[] problem, out byte[] solution) {
            problem = new byte[]{
                0, 0, 0,  0, 0, 4,  0, 8, 5,
                0, 0, 0,  1, 0, 0,  0, 2, 9,
                0, 0, 0,  0, 2, 3,  6, 0, 0,

                0, 0, 8,  0, 0, 0,  0, 7, 0,
                0, 0, 0,  5, 1, 7,  0, 0, 0,
                0, 6, 0,  0, 0, 0,  4, 0, 0,

                0, 0, 6,  7, 5, 0,  0, 0, 0,
                9, 3, 0,  0, 0, 2,  0, 0, 0,
                5, 4, 0,  6, 0, 0,  0, 0, 0
            };

            solution = new byte[]{
                2, 7, 3,  9, 6, 4,  1, 8, 5,
                6, 8, 4,  1, 7, 5,  3, 2, 9,
                1, 9, 5,  8, 2, 3,  6, 4, 7,

                3, 5, 8,  2, 4, 6,  9, 7, 1,
                4, 2, 9,  5, 1, 7,  8, 6, 3,
                7, 6, 1,  3, 9, 8,  4, 5, 2,

                8, 1, 6,  7, 5, 9,  2, 3, 4,
                9, 3, 7,  4, 8, 2,  5, 1, 6,
                5, 4, 2,  6, 3, 1,  7, 9, 8
            };
        }


    }
}
