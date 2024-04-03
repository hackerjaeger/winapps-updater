﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "125.0b8";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "1ba8c0bc016221cd7173c4795bbff05a978ad55daabde40fa34d95da8d0fffe2921f35a478746617dac3065861a932499986ed22b045205f9a2c63c7386b6100" },
                { "af", "4cbfacefa1ba14ace49055e5b0243f37415a2fa928755729378f9b140327c0e607d16f366cc25718dbd1650babee571c8ef9b2a5907b4dcd30b47b372834d3ef" },
                { "an", "4ed1e041359be82c5f3aa1cce21afbfe0efb731b54a0cd935973fbd423117b0e7f30c0d4e68ad5b9f6d58b7bc3445964e3d599fae5aa0cf2130d0300fe4a55ad" },
                { "ar", "c388778060e7bc403820ac95dd4f4f6e69b61e4c6814c77c932e87285e1a7bb8dcf09256195db992ae700e1bc9204b8b8025f22f08d522af5a6eb2f5f7ff1d42" },
                { "ast", "947de5f4aefbf5c2cc6975134de2e6c77afe8ab18ec6f2188ce7d825e5d1bc40f91f7810a0d202254efd6388ea5c63e49fc76c47203d9840f554da6da4c619ef" },
                { "az", "09f18379fee9033f33c20a4fd5c0e6b22ab6a4ff1373982500fc924bfac1a7ecc8ad0da2dcea553b8b42fdc4903801262c3445ceea7b3554189adc1180312e22" },
                { "be", "347792ff6cb61b36ab2418b388fcea0264b87307bdb96ce3cd81832efc6c23b9ad53bc89b96d1a22f14b64ba89e653f40d26a20cdbcfe4df26c997dcdf9e7731" },
                { "bg", "b57315bbf7988122dea1bf1052f60242f7ce4aa1854eb7585e6c9c3d49b1c615642bc268b66382d41f796107c0155bcb5c8033f6d4b4ae5bab918e1dc2ecd89c" },
                { "bn", "6513f2a7f7b4f1422e9da39fbec46f0d36cf6afa032a69fd9c5a8bc56110e0a007b5389d21d15c8abbf353e1cee036f69d25764e2bc76e6568e5352bc2b513e4" },
                { "br", "e6e74ab0140062e291248fbec1d17553231914b8369901c4078e6c34b78d3db30b278f6891ce852a78189f7f3d105fc89758549c996fef3fbf79bb82dbc42697" },
                { "bs", "8508af66c7622afb3396eb8c938455bf09f43161dbaf4eef8883987b7d8411f4ac57b5f695bdf7e01c61f57ce74a64eff861c2be3d3081318d2eb743596214e8" },
                { "ca", "0c3f8870ad6c4cf3388763f59a4a6bdda51cf55220808a6af2235b1f64f1dde0b992ebbdd2ff03586762c61145ed9d09b326f0ea99529c918264ba3ffc0f470b" },
                { "cak", "9504d70dfec4264bb2577ed87ce890a8ea846824f4c064b09b918effafb85ce1d5b92f8cc49bfc248fbebc20ca7aa6e685e4c11757c723fb9b58d40d1dbce96c" },
                { "cs", "85f058d3ef225baccc8bc7a3be28750deeed6b5dd9d1a31a89cbe7598f9a20b7c5788252736d3f7c12071a8188ecb9a6da1b49f51d42cdfd4a60d572bf02faa3" },
                { "cy", "cd348af9a86d73b191e298d4b67358b3c0aaac6c4f3d62887cd7303d923890802b23d9b11d0b194840de176e2ba9f1552213022a62098c34fd193085079c35fa" },
                { "da", "c922240c84b4a10f7bf685e2cc077f53f873e541c7618ab771623a7a068ecec46ec3b7b67ac0099643279be17e809486d82b739d55d1fff69d27f9a816be0827" },
                { "de", "c1c3513410565eb0dca801209b5076d16be3343caa5f95f97f74d113d18e4149bad9b4f8d6cc516dba3db252f19e737b875895a94452ec59a5b95a0d2b468f46" },
                { "dsb", "4015d35fe53889800313f9f3fa0c78b40f14fcd12334856d10f7361ef16df0ccff3afe4c08f06ebaadad1ef341de444e3018b140198a09cf6cd339490d94a903" },
                { "el", "1800a06f1b94e934a4d805081f8e8f737ca2511161728b586019b3f30c4ac8ca26b8e2269500ca3fde5b60d6a86edc55325efe41ed6e5b7ecdd406f25c85876f" },
                { "en-CA", "2584c96386a3696ad3bf20ce3b488e61a773db0cdef7ea462ba80f3d43fdc432e7729c43e7da870b8fafbee219d77bb1e923818eba1b6ac5122095cb4ddb440d" },
                { "en-GB", "0821ebf6075dccf005548ff5e29e4e90cd2185ce93317004548fe2ae1c4f3c0612254ebde7cc166944645f5b82796d98a1760ccc7e45310744948ec782650efa" },
                { "en-US", "dd168a191c97b0f9f11eb9bb68ca2d498eed28541313de76bb40f3ab78dc10436e71e4c0f723cc2f243c8fc2d4cc73804e130fe857094f7a5e21079f71026b0e" },
                { "eo", "9c06f5d2b1d21a19538cb85023345f0f792168300c75ba71a94ab1eff332b21ca539a18a25e29b095be74235e4d0f7ead6ecc945133681733f0b26003417c3ed" },
                { "es-AR", "aa991a6c680f4b9bc910251434c49368f7a198101c08815463955d7e9cd391220babd31d595398d10d0b105b3122d974cea59f1b0171a51aa9c8a1cf63118976" },
                { "es-CL", "3e8293ed07db934df7ea5c056b472a6eaa9e0d9cc37d9315db362ed1ce99eaa97517fd7e719cc57092219c0531b4c26a368804cb1c1b8007c3c3968f7682bdcc" },
                { "es-ES", "4fdc89a2cd7a5bd555916cf310926170984066d08df5b0a3cdcebcef726630c2a74d23b9969b038dd272e09bd37b49a8ff8f846b02ecb2786b42a4692e86885d" },
                { "es-MX", "86920d8b3267cec275eb78e65683ae13d21da10264831c08f648d19c4dca1b60383ece0cc59d4a86c9902e2e99eb7b2910f7354e049b1384051d6d8cd210a4cb" },
                { "et", "28e4ccb02794d70dd2c023428d6fdedb0719e7f96b8190f676a827d1c09c5659e5689a4f8e69da14721b0810e1a780884434406a097b1d05ba8158e74841c0d7" },
                { "eu", "072cdb1921afef1cf0a168c0b3de799651fd9bd46cab05af5bbd88f5c4e96cbd11694868677861cd650537fbf6f60958337db467d6eaf4cc43c5f9f8697e4c62" },
                { "fa", "aeee81be9d2bfb3935aec3f488523bd6227e3923c6859ec10f4baa14852a2eb86555ec3a2cc311bb2492f0725b7c088938c670d7ec36f32c3f293dcf6671d358" },
                { "ff", "4add0b57da358e4aada7f5eddca27ad0af372b83cfbbed0e75b49ef27440c1f22e668cc6a4b97ed3df18520a1b1c2dd60eb2c0ba4eb156bb1ee18c481530f271" },
                { "fi", "5b7aa78089af1a3b3e3342d01baf07e3a166ebc71b451e4cc5c98472b2419d1d53583731c4da841f70c76b57b94793407ede07c414574e08999d289b0a87013a" },
                { "fr", "ee767bbe577bd9f6477579507f05bcac515313db96774a1bfcd8afc5581820109e8c4248293394a3dfb4d6fda98272013cbb30ff7e5a232033364e2a6a007ff3" },
                { "fur", "39b89b2517c3e225fc8dcde0b3eec4cadf48175e2aa5fe8ed950d715c5a11131eea22071b9ef505b03213af034753fd975ef29a793a8ee6587a198242064c3c8" },
                { "fy-NL", "ace898b650c92c0f1316044870f51e8245d7c167108061acf6865c90219de721d5527d54eb76640d40cd451f34b3332286cf2f87d4830a633e49d0861536b957" },
                { "ga-IE", "3f0b20b3316d2acc28f1ce2d3022f82802ef1bde17c3e2034dda528c84ac88507b6c4c84314b7d2fd64f3ae5c56e2932a648f908b3a8d3742eb391aaec724f21" },
                { "gd", "b65cb115272b64acf93ffaba2b26e7d3bfe8afa9308cb8c670d09ebc1240d4f42c4ab54a5a03c799a508d8f614adb74fef7d95344e040beed5a7b85fd6acfa88" },
                { "gl", "8532db6813c1e9bdfe23790535b615be247c91b5f24be80fbe8e2a5a7c81348811a5b95799a7dadea153167095e9bb7947ec16de8e0b2b86631ed89b8eddebd8" },
                { "gn", "c2ac7a169905121eb75872ba783f859c4680cba5bb08568d8827fe0cfd6baea0811e4ef18ab5179bfd5a1208b0132e96d041be59c99bd99c073c403141f6888e" },
                { "gu-IN", "a463202d95b889530baa17948c1b082b90b0ef33c2e547d8e255682d177861a87b9915c74a052cfb1064fad5246ac294977dc492e7ca8361fe801bba73d313d3" },
                { "he", "01a9f5ca84d67e8b851b3725717927c9dcaae8c5be1ee55a203cc06a423b9c15653ad1e3743eeddb92d7bda88fe75253b6dded0f72dc23bf84a1d3c2bb43e631" },
                { "hi-IN", "99828e79b9e6f18206af6901498424b5b1b83c13560fdc974b56d276b6831f68f1bf9c9f000c4e82c728a0700f359bc011317344527161dd0b1c15297c01e410" },
                { "hr", "50752f0699b1fe86446ba40576c4edde83619b7c48ea67c4d75f44fa835504e515cfb174278bac6b7ab872d6c99df5f2f7d49474baba68892f45d8a1cb3e1dfc" },
                { "hsb", "331506cfc8203d2eb342520dbad1ee16a9fc555ae33739e5864c39084933590e440b8a1e53fc89e7b5dde05f2ab9b02c7654561e9ebff6d07c65255ced5e79f9" },
                { "hu", "c2089794d1bbcda436f504f75a4f5193a93a168cf79feb073b429e4374af612fb9d62549ba797407361c965254263cc0f696a5fe7a705733d79723353abb8572" },
                { "hy-AM", "636d7d06d2b5001862c5db7b47e4c25a4f2547b60621ec66312bce0a2e412a8572bdbc59c9f53b8275fd8ae37a11faae37f74cecd64008630c1bcf1a77d47e33" },
                { "ia", "43e783768fa37d4ba57103434c85be06c9148298158906b3c7d81f53fdb5e8d256067f77b8ca7fe04a503aca2ff11256947050a31d455eeaf558a347a1d7c77c" },
                { "id", "d16560eedf8b033250d556509011b19fff9c869ccc6556238a6d0e14c0a75e5bd90fea124da868bee26a19db3d0185ecb4b6bb55685f8c6ed7252b3423e4fb0c" },
                { "is", "cf27d58c0f8a91b7256dc0626318c78ddc2ce22cf66a785dd78be04ce92400dec07a1e0e1b54cb7ae37f381446b36c3e412256b69341fc159805c35fe57a64ec" },
                { "it", "6e711a6c5dabca44117204df7d44cd95bfaefd3ddf1cd8aeb18d6533e45a57ba2ade7a72309a1d30d4d8c96fb7d82c329bc10dd62e05ea7096eb538f1c9eb394" },
                { "ja", "87fb1afaaec5b611126f45f20adbdbdd98b7b247a973f804a9a2f6784c87e409a5d04295b39980c7647d9188f84060f373f0821b0014aadd496a774c05c8ca74" },
                { "ka", "02bd956aafe342fadc6d8133449a1ec29b582668db0927b2d07808961657a1f91c1e6f8fe4e2fc706791470404061aa44b4f2e01de52ef0c57e494e74a3de6ba" },
                { "kab", "25ac51ff71f82ff6ac01d190fdc9878112703ee9d218e755ceb8977705c1b21c3ca52418603ecfb4ee6671d4a6103d0c56f515504f6193278ca77ff499c9d56a" },
                { "kk", "3516c2405ae49b32d6b658ac4c63c5d6337382579dc443ea9fa212a0b5c1af8fa83423262e0f8434d672a77938a222a64a74f48aa5e6d92c6a933a9b89174719" },
                { "km", "247a4bdc52945dae1d93bbad5129598b7d608879de71df4e5042d0537b0ea1f9a490da99dc124fdbb0f59372a4e2cce562684db9ea79eb6261f932b202395086" },
                { "kn", "3834fabfa893da920609e2e2ba5598ae73fb6217c8fa65e54b3b3d6358749a3ba3220d0ea35cea3680dd36bbdf956e19942e3cc63917a8de421dfe817aa0feff" },
                { "ko", "518c622ef8f4e7d4d23a73ca82652c9dfa89b23004a25990fbde7af2704036dea50af96faaa0b761feecb6b72f531685cd3cf32c96113874d95ef08df16c413d" },
                { "lij", "0374614234175fd5ad84b16ab2639906290fddccf374391864457f7cb3f9500ded6a52ddab19a54e8352610a6eeb18ce71a097374cce563c60249453b86a9501" },
                { "lt", "60ac0426917c6ebbecdba675a4fd8e161f2b3e284cf8d61dcb6f3cd6a7f073cb26fbe84258fd4505c0f8c1ed4c5cb871df34ec8885235af5e381605dc19f0d0d" },
                { "lv", "4196aeaefa02310a1c22a5673a607d2bd6c0acab28578053327f7f13a04c96cb3c0dbfed72127b6c070222b92df87db702e682eebe2bf75509f90ec3708b6a78" },
                { "mk", "fc08e5946cc5920469aca9fd8a2dd448b7128cd2eb1543b6909dd7ba614a854bccd9e2c2b28060ebbbd2b157eb4b77808e07da750ce7699572f68ff50ab008b3" },
                { "mr", "fc09bc0300b7b2c9ffa7dda6aeff07978d6a799df99b329f807fb7155683c505d408e3c444d7e5cd6949243ad0f5ce46914c1d2d1614cb62d74347b4e321834a" },
                { "ms", "70f4c83e5612255fa7297ead56f6de769d9a4db4bf9947ac578912124d5e2a78bbebdfe84c01032f66eb2ee000ea8a5a35b84d4765d37bc7d1e4c308a5022d0e" },
                { "my", "c45d82ea62398b309fa0eecc11b407142483b13777c655b0cca449725ec0553ddbfa7b61af7615b71eed6e7f249a692d4719abdd2c439e3dc3aa1d6fc3ca28ed" },
                { "nb-NO", "5877c9cd52d33e2f4310ea8945a7c044b62e7677023afc6ff84a6b2c02d71acb6ac6ef9e5ab5250898353ca31c95a30a1922c24f235ef5c2ee29660827fea159" },
                { "ne-NP", "1f66b2903619a909fe32ac3e9e4f8f7ec324b77980dbe5a7b5e8ad070a9a34f5427e189da47d336f7acbc7d8776108d8e8b30505b20fe24203d42d9f79e44f42" },
                { "nl", "13c0a64aece6c42969ea8d469ae3ab31ad0f3a255dc6ebfc828d264e71f87c26ac57a74205110cf4dc584652d0caa82feea898bdcce1fc7f5fa0cadac13e47e0" },
                { "nn-NO", "00a06aaada4ba39cf5c438d41f57c4a02e86fdfe30e56aa4b99a31f2176d01bf50d71cf49fa6d56655830b4835846d84b1c08bf87e7fac006ca59b663393707e" },
                { "oc", "67c71aebcf44052cbb940fd723136580807e4695941f0e71fce8043d14cccd7b82cac3e8e4e1d41a5604b6929d55136a00c5a7ba7fbaced5e1478c2a01b4a367" },
                { "pa-IN", "cd0e3d1080f5c9b2f7e8a5801526991bb5b90008c803406ae7fcbbef1fb0bd87980e530238876c9742dd710f01a8224d9ca0089a705f30790b9057d1b89552d9" },
                { "pl", "4b0996c549d91247c6a8568fa68f55a882f6ee6f197957d68c59349a81bbcf2a7abf0332c4fee16d09e0b4b9a857648ad6a8e5a8480698ae306d3b9d31f93528" },
                { "pt-BR", "f2b689b784d5ed4cfd3988f459a459363bfd7ce8daf4b7aa88a3ffd505263fd00a2b1781bb8be2d3f73fd0d5575b0105972554190e0ca07e35737654b5d2c9aa" },
                { "pt-PT", "6363a94936981a83a63af0a54b75f28b5ed632345ce3001aee35bab9c87417cbb68217abf618906f98864537b4a48047740dbba63d5ee96e003ca0b925225f70" },
                { "rm", "5a1cb5d2a73c1e6075177707d6c4c65cde7fef8c622b0364f3705a548fe9675654633b2dc09ef25a21ddebb2dd18d1c9083719ba4a72905fce6a87716136ee05" },
                { "ro", "d8e7864910c680a780259f08929fdc1e82f64343bc59ab8c7bc9dcc5e01b52df03a922bed98afb66ebb519083fc4badf5da062ea37201c87689cf2ae37f81aa2" },
                { "ru", "909f3988c9a1d50a2d0593a95f6633f48edcc709d099358c49461ec502d257bf2c45f758adcd86d729180f0b4550cc5b03a5390c0d30a09af4937dd5ae12b680" },
                { "sat", "0546351eed6a81e0f7bb0fcf539a991ce9eccb7e86f4363d1ff77182f29784e0dcc0802d8c58508d5f87410d280f490f73c98f203d6a3a066ff66a4f8fa7daee" },
                { "sc", "8d2006c245d9a061b2a2b34df7d674bef95e824c9119a306e6421f6fd56f2b542099c5f59e17ece8e12f42da0e6079abca638c808702aac5f48180c9808cad23" },
                { "sco", "0b62a9379891ca8dfacbd21b71a322060730da5606873df5f9174176926913172af7ab7d4acd9b9c090f2413f3ade907a8f7edd4b0da06d5cd124ba583f5899a" },
                { "si", "fd784985deb790cae9f0dc171cadfb83c6f0a6b9322669a1bed6471535bbf0f819444210bd1321682ed8b263ff654e319efa804d05c5f48aea7ade69bd4dc180" },
                { "sk", "067ba3fcd34f84309ffe2169f52f42ff194d7fb16d7bdeb82898d806e617d3577df03561b5a20709191804f74d7a9654afbc4ae0d0a3cc39d27c78278c58b410" },
                { "sl", "108901b17b943bedf19d0b49fe8f4a332a8d662d66ceb6b74d649ca4e1cbd312c3ca704a646d654da6b9b8cfeb74a25b1a920f8ccd4c369adc4105ac5d04f45e" },
                { "son", "3dc55041ad88255dbaaa9d8d912b127075483ec94bd7ff39f471d29ab9260eac13d7b4ba935bf57fd363a415591b28c9d05eab56805a42f02f76b66e5e4f4eda" },
                { "sq", "e046ff36e9c377346f3ba932c6079682c442816b773a7040d3b284086e94e12ca38394ec36db53a3b899b959bfbbfaf5172225ddc4c0cb7f8134f90817fc6576" },
                { "sr", "82f437eadb73f19f17c1c461e774b248d8a80572c9dd6a4cbf54aedf8b855abd91288b156e5f8c27a27dbcb935fa050541738a48acd79166fe9d57451448de16" },
                { "sv-SE", "6d0fae0f007c1ace49717ee66b2c23bac47cbe27d90211a0c7ec158a63f055756f19f71a73e879a308f7b2e5f026d80bc8fed199ac9e5ef123a3bc81b3766bc8" },
                { "szl", "7ac94f2fdcf87a18cd76ddc6039711f751f5a241e1cf79dc3698af9d05469810f7301115d35940332b5bc4c101a344bcaf710bdd15ca484e4d90046f3e9b48b7" },
                { "ta", "bb9321a8354156b3215a34b51fad37b45df983a87b0c70169af370b6f48d9e0c4404bb52f5876c52af41cb4de6a3c19a52afde3a6471dd5d22e0ba657bc84bea" },
                { "te", "1ea5d6477dd4ec475b54060a698c40a09f4822d0a5c8d4215a508ec7ff2bb49d8eb8140db5612b7df46a66fc42aebed10e7b82166fb079f50e2dca6d45d66260" },
                { "tg", "2decf06d68ad630d84261c04e0dc63919ee5ccd855c3b3ee66ea2c87c4d03bf9744254d511b1b9c46750f21eb84f75c6f0eb3e594f28a4d98bd8209b528b08b6" },
                { "th", "b4955d110a52d4abd7b30afde28fdcb3770f52ade678fe01656b7bd0d28dd6f4c92e91bee702b86dd615a3eb933a2d1ccea13903694cbd0995d50d8374c97cbb" },
                { "tl", "455d6d438484190c01fb2bbc696cca3f386b8e170eeb7b84d17ba8fab7ad90b36ece06c896508f96f6e6d1f2ab3d583e17196644f59300e3b31e08c256093182" },
                { "tr", "cb12e66378f10b6c4fd2e6f8a4915c96ddc15b607d5fd7462cdc1f48eb9546a8ab62dc10691d57e077792cce99b374c6c156e81b5abe9838f4fe8c06e7702d67" },
                { "trs", "fb0ceb4ebb6dc8c55ddb23c684dbe7be48bdf96a7098cf11a1dd99b4eb27f23d78064ac6fb48d17c06a937a4d32eb14560e42c7d01c5251bae266924f9f21689" },
                { "uk", "0470ded1093f565ac951135215e96ce13ba05ea3d6a822baf2270dc0b703486e4886eb073d4567059a5ab2eab60a4ae90c24d2aa74a3242b1d5e9e3d0da442d8" },
                { "ur", "c511984d5689c23b817dd352d1a9dc8885e13da0cc434489c3a67fa4e1d84086e39d7901c2890e31636c9f1e1bb38552ff6697f41bf01b0b2daa68d107d5f132" },
                { "uz", "2b10d8190ba32bfea0f34fd03b4a98c05bc9653cf24fe0938bebb71775e4b16b457fc368519c68c6e2e82562f8f6e5694be1cabb3d837f9cb8defefece965a59" },
                { "vi", "a1c6c46c63ea025a9a7a117e214e131f0c315ceeab64bca4ff031d2ac8891ae8e495505ecefe3a8045810c2af553f7f6f87454946835c5f346be486d5aba749d" },
                { "xh", "94b8d4dd5c308e8e518300aa8b486314cad55b0f2b87defb73c0c139687a64e797b2ba6817499cb43d07b726475112ce6100b9c63d109d3a283be226b3d0b171" },
                { "zh-CN", "92cc8c2082ed39666998942c2fca719540e4b968b7e015e28d34ad2e22638d29f461e81ea2c3cc8403ec7fedc02a2e086fd04bcfe620ba9eb048b2caac447c34" },
                { "zh-TW", "6baf1aa9f1de11210b19a650b23ecdec65727417d8bb8d16d906d2c4c5fd9904f0185ddb8c1d57bfe87680875b3bce8faf017bd38c3bcf37de240028c87369db" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "5a4f134fec0e01c164fc6ca06790daad820e5b0dc330dac58e11b393e815e8a40b9463870014c3d557466e7eb0da897d892fdb5130e2c08e7b3fc5fe7a82da05" },
                { "af", "5405e9d12d9d8830de96a85fe4524c05771849ea84525ac4c8b8ad890256e18c72ba1594592fd7120a8efb2aeb5b35763a5cb0df1b899c4730247a3aeb2d1ba2" },
                { "an", "2798b8e1b065400fed0783c1e9ee5c10754f6ce7b12d9d0ffa9d814e24fc2b2b986b405a314b2df7f61319c2eedc3e76edae4c9e4f4ff2a6ca289a8eb548bbae" },
                { "ar", "9e116191641c1a4894335a46d893111f88439aaed484b819f89faa103f6b05acc5317b8e1f820f53db6f350127a1ad9bcb7cec925eb818490aa9a80aa38d6b31" },
                { "ast", "fb2af8588f0798a5e81b96dcac55f68965ea4ffb7e8570f4a1eac8721bc9bdfb522fa6dbcad10c50d5ac5dcf099ad4f8124e869bbec56e228494e6f9917804b4" },
                { "az", "8274b5778acfa50981984385777e0780b800f4aa0e5e57bc5696b9971c258408f00ce2e0a4283ee976305cd5adb150b94d545cd27675bff0f14edf6e6e6ed135" },
                { "be", "a6ae89430b79f4b2e1aafb668a4b19a44e6078807c89b0be28c78d9364165513886e934dcc0cc7affb25f43ebc44340e70e6491dfe66ef97cbb58625d85c0242" },
                { "bg", "f1ede9fbec6ecc68317576caace23565e340a93842003449639cc3b0896f8a958cf59873dc7d6d10530d90a7e7036097c6067244898805b74e967db593d910a4" },
                { "bn", "b09fdcced901a87832ce1f82a3fff0ad344d3ef79ab3796b45f489d8fcc2170742106f9dd64860d1c357fc7f5d8248a54b6ab9a39a4bb3b580fbcb2c01f1ab26" },
                { "br", "18d596659d0476cab41891d4fe56ae9817109ee9c6d6ec7b183ab6e0e54a2fadb731fbb327d792c34286a4fdddf90a58c94641cd0aaa50481feb6526655de269" },
                { "bs", "8b320a01f238fd995f49d7736f94e2010d2a1706164d0f61c56520da05973514fd523fd4b686f6c20cf0bdbe4683f6f2fb5aa4027a2a170b9ccd719cfb76a9ee" },
                { "ca", "e9fea759b9e49152ede4bbc14a42160c8710ec4d484ee2e84934fa5e92d3dcd607ceffc7f021542d2e0e2ba8589d7db40d7d1a30fea4d5779e233340295da4ac" },
                { "cak", "332fe8865818a2d0f50161faa87858da1d7b0d4d2a7441093926b7e3adc856c84a4469d59ac74102cd4ecb37adff9f52f01da1ee8c315b342932342b921792c2" },
                { "cs", "ae9715a0e2b30e535d2811bca469dffc2e5be1a673740040353d2f302ea79f49749c4ebd0bc8253badaccebe5d6446cabc2eccd4a5b1973730c5febd0af1ccb2" },
                { "cy", "b93c46530601a0af16b63ad2839bf6873cefd9655847cfc55511d467937fe6e856edf50305f1351aac184e8045fa0d75d0db002c989387b9479b73ef7df3d878" },
                { "da", "d435606cab36d47a043ec614bc0a4f03f944fd023ed73b843f2767df68c977a5fb7bbde5248520db29392ceefe9cf51605c6cc4507f968cda2a477048b17700d" },
                { "de", "8812674487cb179918271a690d4f136f33240d3b8e3ab3dfd6579dd8c3aa80e4e27440e3635acce3c47910e5ead17ff3da15c0af81ed73a70725798e5208918f" },
                { "dsb", "dabb0d11b320e60ae6f5909b798dd3421c159e873ef54f15d6254b2d7570b201f005e8a9c647c404c47f79450a7d02686efa7fae620b8b7a04dabf9138b4761b" },
                { "el", "c7b3c989b4e7f38eb58373cde55cb7577a5763a51da4e3fda7895703966e8c99dd4210e000acf17697edf44b9316ab4cbb1c173ecf6666d14addc78089ce19cf" },
                { "en-CA", "276b011be84d2d048d6c850b919a9cad748ca3e515675d58ec1bd3174182552fc752bd675ddb0867dc6049721e84266976803919f9c687850d25ecae5f8eff94" },
                { "en-GB", "d45f327aade1005c2f6a1d3b120b13597ebb38cde38cb56bf8e1bf233c0b6751b1c6906d7971665f007b9a9baea65bfe61ad52c6f7e468c66e85d88fd6c443ce" },
                { "en-US", "20e858020b2e275327c138b1e68a2bf15ee32ed675b45520ed2c53676f80ddad703ad2ce47ef63d596c6a80f995dd6e65b8ce7b7d9e35e3cef5272820ad6c65a" },
                { "eo", "635250d8ac6bf58be26f6512f520dadd3a9e32b45b8f6a46a5a78d8a6278855d8f3bcf5d2bb3d901d4ba4f19786dcf3a0dadbc7ed1a07dded70062be86709e1c" },
                { "es-AR", "e1d96550de4dd0e3dd05f53cf63278de6f559dc9d32e9fb6d566ae4790fbc76c43123a0d3722a0833638e47224580e4525ebd929b85882eba4c269c2dee83677" },
                { "es-CL", "d60961c877a1fe76d3969d9a9df60a58c04125d3eec085d934ccd9546ed0739034a9ee1cbecb068cbc12b5d4012d3bb4632a6e8e06ee05a844518a9cd777f089" },
                { "es-ES", "18920df3037bec321fcd69dd3fa4435a34782492a2f7a92c65afc4eb179fb100723ab02f44bdd95e029eb50212fe9aab625f9b7ebdc592ca981108c6dcedf11a" },
                { "es-MX", "91d44c204ee8fe390998b6f7639cdc64cff1a6302ef52ca17a0290ff9bae4b909605c014f627944f58c23b99527cf955ce5e9352ab9a213045ce347bb8c31b87" },
                { "et", "6a2fb04a81df7c312c076b6e698482c59418af95242c79d9ac3352408cee52c1ed2f1f23a25b8dc7215ad93e6d1a59590b91b0d42a3b763d96f6faf7c9ee4541" },
                { "eu", "768e3b949e51a86df320625986c79e0f65f6396a1a1d70795b283634c8596898bc6dcc8ac406bfdf27dcb6fe60b4c188263868eb253367f24a4123d80284a668" },
                { "fa", "88d09528c45f5dc7c5693b69e4803b312d767ac0a805d47d54ece44574a35613c22040ab8e212be827688a9f8d28f5a966817c4e2abdb88ea28551642768f3d3" },
                { "ff", "53b798f033c4a4acb09ffbe278e64fd79be2181a132a47353c6f52e558ea366485ca8ad8aba2043c8254cb1db22e4044a1c06fc8df78d7c4132e83e165432130" },
                { "fi", "c81d0ac2c2f4bb31c18e6f27bed8da3e776c67eec3d5f5591661682157055ce8ede12b1b20be46c2bbc75a88e45d5bb8d102772116c712b0840670d157eaec00" },
                { "fr", "101d39c55f27d6b397bf3f1e33265703578e24309535bc8fa0ca48dc268e917e377e35aee94299cee9dba26e0e78d12208e6b62fe9e0e0dc4c122d17bb547954" },
                { "fur", "442367072e8da74048985f3fe7bb57873bbcf4d5147195931598167ddce923e045151593f9436e933bb9d5b98bf42e567a91814de450c573625c22c08bd4066b" },
                { "fy-NL", "4b92a3b7e1a29f2fddf46f6cb00eef5d56106a7c6563adf7c6b6a0f495e01100c4a8f0fc265bebd6d43beebf31464803f631ccf423fbfb8ad0b35086a76e7118" },
                { "ga-IE", "1912743c670f58fde4853699981c6f129c291263f4779a156d4cc9d801c125b19fbefb662257cf1a748a72213f268c532cae6607afd3fa41dfca4e9ad3cd89e6" },
                { "gd", "dd97f454ef9804fe7daa13595bca8b2393e86f3d9ddc434f9609894c7dc7495a8097a33d127b2c5e12c42211946d78e55f3b5a983eab868351e6e22c5e99b857" },
                { "gl", "f244d57fec718681f5f51386f6e65bb20f95ef1eeb35e3bb38ea5ae13405c91a97d0aadaff90020e48921e1c1598800aaabe29d8e00aef9508901d6decc02506" },
                { "gn", "b123ccc4439ae9de203cc5230c2576d83227e4bd38e889a1c66653ec479f777b7933fc1f2b4c67e41e693a6e61942b6fec7294c962405cf4e6e19a777870a833" },
                { "gu-IN", "a556774fe539afa509bfd956362416531eda5975737b7bceec95c4844a9ce9ceed6e16fdf1a7e288166378b4518ef4340e1f427692777eb8a3db83136933950b" },
                { "he", "a4bb330a49db78a5a39417072ae0fdd0ef4fa63fffff8d2061ce6f0a51b88aa96a36b933e9091ba7cbb2f911e5efa56a0a6b9b8bae8ea027ab82d728ec280f51" },
                { "hi-IN", "5cb812c342271bd143febbb9f179cb24436ceb36931d9e366946ca7b6edfe536f8f0f896f62f6817ed869ca0887633c5df595055781b7c5d857fb113aeb5aa0b" },
                { "hr", "7963423f6cd377ee193b66d4a605ae0914ab2383c358f5f9a0c274d615ef1a5ae5ea44ede8db8559c9b4e6d6d5be9c2003fa086960959c8323bcd9a5d9b05100" },
                { "hsb", "0f7a051d92f4485fbd8e51a00faf7e611b49db1d6708dd9240eb7d6a50fa485254657b926679723732ac39d12010698bf99217b5526be67ecfaf5fd7b90ad147" },
                { "hu", "c7d897c6a5aee6696ffc1751412e52386adf012841b6ce8fdf476bb2dc8d5572c0262025c0f1a8fb6e97d0651fa8bd2e3568b68940c3cb3895d2336fc9b3d564" },
                { "hy-AM", "0d9d043aa0e2861cfd64ad81ddcca1a3812403109798ac6cc670e766f6a5c1bace902b4e4167550aebab36ec3a437ca211d9e283c2b2a55ab2c7e522d9547a2e" },
                { "ia", "5afd0907fce0be0080a19fd3b0e2808629b65a1df4ef49025444318f9364f502626ddae88edbec83469142b010583d5e769d50ec42a283ccff8d29fbcc332a30" },
                { "id", "5f73e6e856fdad06f3a373da8adfec798b74dd04deea134728d3c2ade28cc940d64c8793b0e97c45332ff2e9403c1f73a168db67d0c070552ad7cbd80517919f" },
                { "is", "43aa2e88973afdd382a1c4aa5ffdca299c4e023e7586f8081fd6a38393820bc14e3ad973426384efbdac97952ed3b76bcef9f9a5b941633911bfaf28aad2a4ea" },
                { "it", "95005b1085f292aa43ed167be7420bd780d698341a9dd31c022901e9d9692b0cb006eb646c9ca96b0ca0e7874d9bb1814c8beb01d0e85749c9b78fca2c2437de" },
                { "ja", "ca30e2e5a275bb87f00bdc6086e28d2529f3644d8c69afc5dee0b56a04588d8ef3c208f078ac8cbf7efdf105223cbabe5befe29afe96fe3ce9f7dcdf35f05578" },
                { "ka", "e29f3c07a99d1691c2e4356411b958132d3fa34e5f0c55873a23acbd7e046ba8fcd9226ee1d20ed5510055fe89aaed19a773ca60e7114845dfc4bc3f5a97491c" },
                { "kab", "ae7a83f166c9a8bca750e8d435a1f620012250440479ccbfa66b13bace1cb36c02d602b59468d5919807014925f6e055f0d98dfd1d35985a5fa8b3382dc400db" },
                { "kk", "62af62682d9585f006d859bd079ff80cb8b91b27d63d63560f329d9c64966066b3063dc483c347ba6ce20988619c1a7007a7aa013ad97d9acd5748b184e212f3" },
                { "km", "65d14d7d6a41d4fdd1de6f718bf0acf903b682109c7818097a0a27fdf395109e91477a516c45d995388a2193d061d59d5e951c6bd056a4d14ea2258bef4acfff" },
                { "kn", "c381f25bc8f0ef4678d91e91faa06f01c95135d8d8ae002299423215949ca79572ed31fc93a167ec4ff605df8f5672bafb60be094df5291a2620f40f137b24e8" },
                { "ko", "3defcff824102359eed35c4725636818489d54ecace4afa7b5490054ec233b1e8725743fc38e4e4c1d257a3f0e3222b620372d43dd942cb166c5780c2269de74" },
                { "lij", "11b3e4b0883037d6e0cec45668b02bd2458438f1376372006c4ecffaa44b9860eb13a4a6625a37c12d0723a06ef14ca30443662d1ff7b2165ce39f0448063a4a" },
                { "lt", "1d292a68c9a5e8be7ad277e197d6b1b716bf911485046f512efa485ec76d3e5378a5126291cbdf124290b7ffc107f3d91ffa994817c576af2970b255f76ab16e" },
                { "lv", "549c98cc827a65edb0a4c35b616688553d15481d924704e2f3f6432e1efcb65bb17f7334d732e4b0779dc63bdf9dfe24cdda1c3d50445ff8f425bc7f03a46f52" },
                { "mk", "af6ab83f4e896ec14a47e83498040a2bd28baad31ec20463e164282b20389f751cd939fb1ef880de6cb927d6eef4f4aa4930c8c3d68b335c39b79f85e4d12175" },
                { "mr", "83a88680d4ccd272033955b8988609d11c90e3a8accb546e440baf22eb043bda5059a35facc6fd2ab1262054ed2a4a55eca8f4068326e49d2ce5e41108376eda" },
                { "ms", "7a058c4581a45060a103f6b9dae9576997f2faa311bfa883608290346dd4f72ccadffcea0f00773ca5530df1915d01f25035a43c04bf011247d6374f92d08905" },
                { "my", "98fb548f48d2b71a8efbc1d3cc4f7f863bff7bb33a7cb71039feb7d9967aeceead23c85639a405bf68abe5103c3b07fa4e32040855e18c8260bc6c0a27dc43bd" },
                { "nb-NO", "039fa69fed8070c2adafa58f16de09e8e7ce8b0ecaf8aa5e4df0e1eae0b6db10b979c65c65e836efc4cd6d86f0ca64fde26099fdd2c9090e10a3d7b3aa377a4e" },
                { "ne-NP", "eddec840b684ee12fa0e958f1416a96928e4ece798ee65190851344b89ed995498a197c40c15f5305d0950b13ce5743fdf7e66d94ff7e477b68a36e99cf6e894" },
                { "nl", "7ac701d70b27c19f1ff8146d7f359c9306805527448c088e00084ac2e1e3d4c835e3b9e3f8d19ae99c5d878ad32ec64282b0e2bc861c40f01fbd80602ba19745" },
                { "nn-NO", "cf52af2a7bd1040a4840111e4835ab09052713ad618f5d2e2a660ed3a7a90242d92b3fc064d4ad1effdd45959d61e25d17187804d5dbb911bcc997f63d4ed2e5" },
                { "oc", "db6fb5dd2a8c9b495e1588e537a6108a288d4aa2728cbf4e998046d8cf3e37b4a5d1dbd48cc0a4e7ac08eb9218e506e71e387f3d1a8c96ac837cfc38a2ddcb18" },
                { "pa-IN", "f892147deed5cbf1fd7ac8af1972741d1c00b4c14d1a176d23b895e8d2adc255a8db39101998bb566e223a62ffdf3deb6752666c81b46b62fbc05d57e5396617" },
                { "pl", "3e014d86476e1ec6946358921d75270fc371f73447c6b718af6871dd27145c1b9328dfb0059b8ae557165687a7ebe39dd0bcd990adaf84f42ea6097c518e937e" },
                { "pt-BR", "cfc812f8826fa93460711f83f37300b5fe4b928a46da485fda952f2f421a13f44b0a2412e0da817ccc140e8a064258d1a7ca09ca4cc62a550993a2ab1bf2fda9" },
                { "pt-PT", "c6f145ad480422fc514bf3562665d65e57f90b96c1d1c9faad7bda0ed26d33744ea2135797c5f4e529219a92cbb64560ad04eab3692716594714c1732a37b80c" },
                { "rm", "15cb9d1bddff5d98fafd4be34a9e806b596ecfdc80d2d4a94347b7b02a99eb5167ced2fefb1f147817534bf437679e5a77e7443eee58e46468ff4b37e46e5882" },
                { "ro", "4f906b3c2216ac3febd9e9a2a9cdfd666c44af4e117f1c9370a0cd3d52c8266faf65721d4f53fa4687ce26227101168c488ed37028b82bf71f3d58915451086e" },
                { "ru", "9180ed4b7af1f8a3ec9449e4c4346f2974781122a93c413fec3e240bb8829eef66338059e8ea1aae48a43dd0e1e5e1277ce5c7501c7a8fee2983edd9fb13e407" },
                { "sat", "8081fc5f1200ae30123f428ca53dc618bf757316acefd1f30b396c01edd7f4f9ad7741b74815f93e380bc473af6a987a1d3bc6581b090222b99c8cdb8b9c976e" },
                { "sc", "8943887470d658db617ae032640aa2d61e537beeaed00d4791a250229ba5409103d177e8f361962e53e6a08adf61d1d094a117dded4421eb148b6925030fc80e" },
                { "sco", "b4c93a32846fd7f69cf02cb91b2f13b03714dd1c0b5a1d81b9f525d522f4d320c8aaff45d346f3edf4b4a4e83140e4f8286a444cf815ef29e718f271609ffeba" },
                { "si", "f84764d93214bff7c53caaab9643cf37b7212416fa74f410ef1157317928b887ef4fce7ee35ecbe8f49f52806bb1b191adc4a5b2ea224b5f71256376644acb3a" },
                { "sk", "49f5e85d8dbd1653bd292b143eaa6346456c35be3add00500c7c90653790d3a27145bd4264da45b7166e3627dbb9aba066754019daf9715742b5b38a91ad5552" },
                { "sl", "a40d09cb093994f7a0bd4f13e1b3a87155a05c265ee2841898405d0176ef26bd59a7dc99661338b4c33662866004c4fa91488e35334bce23670df1298b03dc4b" },
                { "son", "360e001207806a1ab5978f92eb69d87f7b4f77de64bb75cb341d8778510ea390f94f79e34e93cc0d2fc4b5439188f1b0bd799b10abf0af71c147307de7571848" },
                { "sq", "fd9344b6acee1c70e33e1412e56cf9d2321ef4d2aa12bc5a8ec904b51470173e74398fd3420342a43cd9f105d238e970ae93276d4d0194485a107a3b770b5b76" },
                { "sr", "cc65b2ed47b361238e28162f99154e63993410518e3dca98c10e4b2bca7c761859ffa68c6b7dc273caa8c7756a2edf7ee977df805b1d4d9de26a1e62cd1b2659" },
                { "sv-SE", "1d5c3de75ce4ad4baad03ee9991eeb4491cf559124527cc5bb75bff3a60f15d23a234ad50c763b95144249d43fb548eeac5646fd6c46c24e9243f5ddb2855b73" },
                { "szl", "06322cb354c7ad016bf090560adb03df14d34fce5007a0754d1f08acc4d4f77f7de55c39b85f965bb271e422d43969beeab09affa2b8c8a565c4bcefd754a84e" },
                { "ta", "c98e58238c27f2c2db1f00619044b707083c1d42621d1d00165458b04a0ba871ae7318eb1a7c3d06254eb36d76a600d57fede3c989d26a10813afef526f5671a" },
                { "te", "89387004cd7cfe177923e7362349cd001c860091139d3d68d7c4139318fceca9457b1231b7c91306bc4f77915beba805167be5afa208fceb85cf52d03874db85" },
                { "tg", "d957f3ef418f48230ca04be6d0b2bac72f3790df4a9d54443d626573378d47c3e53abd8dd84ef6fbcf725f20dfa635b16df8eb9ece46187b8251c85e30d5659d" },
                { "th", "38cf584e4a14660d38d37e012f7c9775c647ea7f7261658fa6ecde63e6545b42551eee75ef4922de04e9a95cec6968a8876ff95dc13ee64c46eed43f67e378d9" },
                { "tl", "7f071464d16d3eee35b9f6bd8a8814edd8572dae46560811e7aee2099b0beb196944865da3c8509d44e0d5be94db79cde72d369bfcb48eccf7e17cd4bef7abde" },
                { "tr", "91ea1134a3c585f044cd58afef09822a2c21b1a5e2a768366894880f49e5dc9c2698f7ba93b2c266251606e5544ba4e3796e3884088776e7f7f13789963ba8b8" },
                { "trs", "847f69c31659c0d97d6950b1b48f6b376640c1f6f30132d8fb91ece778fcaaf3d92936e125b243edcb7f848cfbd54a7dc736157c264e564321509fe8248f8225" },
                { "uk", "479f75979c6123383298afc65aa3daafe450b3e21824e9064f9d772992c470e91a89cd255833bdbd4b06ef3947fc06b0fb536ab65e170aa4a7bc498f712a51f7" },
                { "ur", "a557eecda767046d51fd46ad8cdd9db85dc3883da15f261c414ffb870062dd318b14051ec21bd27283ce092f63b32115c604916ef922e3e8e34a1b9de4761a7a" },
                { "uz", "db01f2f9d2d56c0934a2280806a806df5e664429161fb08e6e6eb5196d95c7646c33165035d956e5bc0c2101bc80c2d6f39035021d022ee329805dc3a6c93c73" },
                { "vi", "59a8bcffd9f198e93432e076344dbfda21ada82360768d9ba7500c4a80a45de98efababc82473448b27bc1d59627f83a1fbb565e6d6ffc0020066f89b07665b0" },
                { "xh", "302ef9c741e1346fa38813cc76f14a8c184fda18ecd31d6aa0dfcd80cfefa3c62a04b631bb645aca1800d69cce8f90bc00f337fe88e7c8804ce473c211d52d1c" },
                { "zh-CN", "eb528161378b07aff580916db85eb6715d34060acd3d51e08633f3da4a807d13a2f93abf572607b24166daa2cc717d9c505a92cc0fb84d9881a1111d6b44415d" },
                { "zh-TW", "8e3d521dd509937faec960945068aec136214659e6957c914ae7d5dccc9d645cc3ce8843e49d346f10457786fab990b6d06ae28d84b42589752148ea1a09c761" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
