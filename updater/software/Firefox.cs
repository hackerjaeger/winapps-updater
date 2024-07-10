﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/128.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "1bf20b5b1c5f84a291a82360411e0400633e7f2a75ba49c12f68931a2d11dd897deabf80084b82b2fd29ceb240fd037e6b7d166f2c8284ca6c954e39b7dd1ea8" },
                { "af", "bdf2fb0bf4402ed63843ddbe3adf9dafa4480d284b08cb2221a4cee720edc9d19fac78bc8a011a49a883c61b6f07124acdf20e70ffad666f17ae529dc9a206f4" },
                { "an", "adb9d520af64c79f1a9405255e3812fb0a88817e190fdc6088ee324d88623896bc8acbba15ef9d2fb33d18731b6b85c992e25737f64ae623b0984a807199e4e2" },
                { "ar", "167443358e34325e5e50eb95a96cfd5559fc8a478bcfac53aaa7f2f4f2e26957891ab4cb12dc75d14c0fabbb523a1696490c5fa5c602f0a166c9bbced2765b20" },
                { "ast", "98b6d2a2ccff52872cc9dec5c62dbcaa3852361c41f72160aba0392f9503b9a9469f5a076f42279bdab7597732c4234d1b221615f893e9460565853676445f96" },
                { "az", "00a4f73aaaf2ff07f260204fc590a3eae5ab03bb865225e8a7aa4e8616917b8c1fb3e1e518948e560dd6ce6ca96b3775f21251f1776fa67730733670ac314767" },
                { "be", "0a0698a7251cb95c28e254a29f72fd053d00635db79d88c3f184760b9afaa922707670480bc846a08b3925f4e3eae0c3865e4f608a25faf8e1a8008d0315ce30" },
                { "bg", "8d576a7014ec9b8d8bfad1d6341b723dc0db7fbe51c4915e954fd5b4279eaaa7f72d808854b911af4524a1ea77b604cf5dc5069bb56a6d71a11c37be439c3a53" },
                { "bn", "1fb4bf07ead0b4ba73ab4708865854e3c478503ee2ab42c0aeafae992ffac3f7d1d8ac03379d5a7ca4e805cb5bc7290e5e8c0211bc5e02f35eb6c02374ae40c7" },
                { "br", "f85b388d0c9fb29d7fdabb02dc546afac32fcfbe7db47ca7bd4636f2bc5d907b1908bf0f466af73a5fb7f3e3914a717363e2e7b2f1c8804088fa291272f91d23" },
                { "bs", "02365f8a6b935c17c23349f7aaaa7a20cbfeafa961bf301d445a58f853981a3ba7352d772f6aa7a321e8ff45af6ecbb5138e2c53359d729faa36656683347c90" },
                { "ca", "61deb4613a89c8e13e9cf0a5d024285e236152893ec14623b5cf5dd0772c189329754d22a64e975d37ea4a48bc01be07657fcf53a6fdee50c1ad00e90c9e93e3" },
                { "cak", "61803ba1aa2c3a2fbc503b5f3f304f529f39735a921ecbb2cfce335d397e7f4e0ef9b7d83e600af926570df6bdadd0f8fcc9db930343e2614ee2fde84febdca7" },
                { "cs", "4e1f83737d8e7e6a5b17bd67d3a55c89a7c853ed187b18468ec4344f29c43cde060a6a9eafc5f86b64be722709fdddf287cac75843c9ac31da633986bd99185b" },
                { "cy", "f26ff2cae9b950508ab3ca4fabfd9e8f0338749857f060b8df9f481979f0b06eb5d59ba950532d9b153f7323382f1ed946221869c6ff565b2405ce9348c2e1ec" },
                { "da", "7f653254a75c1aaa0b61dbe56eac08d6ef225ae1713cbcf864738b789a71ffba83ad84d39311a8739533d1bf58acb1751cec8f48573c5ac72652a55dc6b963c2" },
                { "de", "04e267e0fa6dfa9208f7bcb34c946efc82ec9ac467a65b199b443d49a1e1229a87fd23fd1e134e05a62ed91d402e8e66b5c0cf2c328e530676d3ef5db4d48fb3" },
                { "dsb", "e01fe122a55ea83f7990496a605c944b77e9f3157843fe8d53e4d3f6e49d7a56f0fec5879a53b26837e143d313ccde088bda44d099437b9fe1ce523a85a4809d" },
                { "el", "51dbc7890a816a7e6f0ec4216db74e18616779b81720d996a63f27f7bba5cf1be8fb02931ef0dfb4192ee8c8ef6a8c1d619c68b4702044a8ec997f48f3f9b53a" },
                { "en-CA", "f5f5af2072f58a93990022a9efdd5973f27a3560b359ff27e9bf9109ab21a978c4731a59525395ffb7f7f1e5a0ca6535ae7ee609fc92c02f05841b567c73c169" },
                { "en-GB", "b7f509e39e9cf47c2befdbc985a16ce842910b368240239be8cad68b4041373696041340f46560c5749f47243fef1a640f04329f5a096692a00a7fc4030f1f64" },
                { "en-US", "2423319633cb821e505541fea27e1f0224b7b9c6155491dacf0a35db469fa61af70d50e95707cd72f530c5ece56f2fd5e386bc485f33a50f9df8bc16c27f1733" },
                { "eo", "f5c636ae8ec9fac61122dff432b9746fbd380746a53ca706ce45564acdf680662a24f643f594fed379fbb3ec601448862dc0b9d153865ef39c635ee2503999b6" },
                { "es-AR", "760aa434310968691fedb3b448c6e00f8351047ccecd6cb1a872d20e1ff57b011cfcf8189d1db0f88669baaceb51b93e61ea51392c5ca4c7934be633cb99b411" },
                { "es-CL", "47a9b70f2e3d88e6e5e54b8ffae54f7740012e5d91a9b01d8988afb9172604ec24cea933763bac03bed02449d891e9edfe5137f26693b32c8babf6f332e21d19" },
                { "es-ES", "f2ffcaf9df250845910e0829ae101ebacb5b9b9a7c588fc669d170b706cefd5ea6dfd47eebf7016a6cb1a02af68108170928715efc8464a9fddddcd40e1a66c6" },
                { "es-MX", "380b4e6a06e61b7c5eb0c5c364f2f4bec93103b5a1c20c0c702cad1ff0ab5f079f8abe0b0d3bf95442ee93ecad4ec196dd6d055cd43f1d134eeff7b2eed6ef0b" },
                { "et", "078723d77184b549f72f0c79d3be6481678f19464d39c41e294245bcee6330676f96d09da64b7ba880288983aab7537866c11d65c535211383294d6b0e1bd84f" },
                { "eu", "593928ee8fa22e4313a2f77e277b51cbbfdca7350929b3e46376fb1e65e4b8ac03f819aa9d5d29df404546e28fea370d2cbfcd5263b2336a76bdc6cf41bcc30e" },
                { "fa", "49b26b1a9db5938c7146c5fb93645c9204d35463f93cecde10818b0a8c17b1fac01bcfb25e2dbc4fcba40e579ef6bf19463ab0564bdd4b86263476f1ee8eaf9b" },
                { "ff", "99cb6bbc33c4197c78bd64a047fee8eeed4ba123e311070b94a8bc5537fbe0300e25f1b8d3ccb101e87c18652020e5bfebeba61aeb6e036341f57f71512113b5" },
                { "fi", "955ccdf5a1c277b2178456ac46e8e9ab57e78f49de3c0322e3f4c3f1df61ef78e62ada206a0e2149d6ec5812c6755b7d53b2d9468700ee45ab0f7b928ca77770" },
                { "fr", "b7cd750aa35260c8296c2c160c3def8c47c98045cc91c90c951aa44afa79e627f374caf4fc661c33c367657795bc211c015296d0dda7c98fe54b3087edbca703" },
                { "fur", "35dd1de4dd3f93cdaf8f39c5e0ea98a64358235623035ccce5bb0e20b13b019efab8c52cd97de0cd77441a8f33bafc78c2536b5927a4b84c1317673b56f7716b" },
                { "fy-NL", "cf514111d430d5e9aaa8959bc98137df64c59a3fbd31410fcb74528d25fa279ec7f5421b16257125f6bf254ad6435de76c7c982787e735910005058e8366a8e6" },
                { "ga-IE", "e7d6d7603e016f07f074022a13bb3a534727fabbdb4febfd992a221d23571d2e4e184b34c3fd2cedd54cdbeb99d13aa59e355c14aaf6865aa38a638ac0af54c4" },
                { "gd", "19e7dea8fb741c36cf8d01053d832245da1d414c40fe446ab00d7b32e7af0241ec7d428cab38e52b93b0908bafc19dd8c9faad9c0f68ee574f5c5ff5639d4297" },
                { "gl", "37482415a0dc67fd8e108a96a5250253726cffe6be593d92876b702931c12d314af3b64cf8b0a30c972ee961ff118777f95adcd0a4c3373231e98344d5348d63" },
                { "gn", "a1178f8d9647e34997c5cdbd6cc139f64fab319feba6a2638c8ee7b583434d7f353472a5688043b3f89c2a7fa9e3d0f61ed18f40b62323572b07d22680812635" },
                { "gu-IN", "6297600fc57cb86d0b1b1dfaf133854b8ba94e4200cb048b8b81c2fa915830587d38a0a7d88440662c48e918f1ef90bf4906e4e7b58dccda843723ef5ebc7993" },
                { "he", "9d65b455ac38486250d8ab4bc40dec63a9714e2bd4e1be23f0b9702d1245e622881b3119745a596b04dfc73b0691fc7fd02749b189224a38b08e2764e7ca6f60" },
                { "hi-IN", "527d82d3d5bffb02b94461eb151418b14164a2e90758987c97be9af41ef685e6f8519f92f97b49c3bb286914ed4e14c6ba567c429860fb49e906a38ec3f4f320" },
                { "hr", "4881af8404174d8f111056efaf6b7e6542901f01c268437ffe0747fb1d4627afd11b8dbdb938668751d04f430bb0e053eaf9cdd3bf92f13a8bc1b4501c8ef882" },
                { "hsb", "465756592447bac89f877b16b53f6704985ec07efe2dab4756d1c1393a86ed02dfe4e07f9136c7adfd32813071224bf7ca7b888e54001f58cc3d45fb7cbc9378" },
                { "hu", "bb4a1a5a6e29f4d3b3ff2d883f0e7804a20340f808b7c64ae50f4561d41e35f58d75ea8dca7adde126bba2e266ffa8ff222ac3e8d78e2fbf4abc04b6aec30a0c" },
                { "hy-AM", "cabb3563c12163d6a03f22ebf44e7e3be5d4104229c38d559fa69ed14d66685275d4429607021834bb6558c168b272d5381744a2ee8d8986d4c9ff040c38a553" },
                { "ia", "83b4c46179f3658bc7f6ffe9a76ff4ba99f98797ded01657324ef2a8c165301c2f6d6a8e266ff12a54ce293d0c99dda88caa751c6ada41be0b5a6da867f5f8ce" },
                { "id", "28d3f2e6764b5fe49ca98d54315216fe46618721220acc63e62c53407f7dfd1151f097055c4fa0e4d041a951a0306f0709f367f463e83eb3d2dbd82939fedeaa" },
                { "is", "3d5d41793212bcb32c618e24c5720ff41843e164b64a96e2881125674efd73217c742ca2569c5d0d191c68e19d0aff4c1698220f203e0471289a37532058719f" },
                { "it", "0cc01b7ba7a2162ff83965f412120408237c21c3876b64f0b886d387eac2b437e3677b6c4e74386a7f3eb6577f70a693c78d9eb8cb12602fa63d2796f2a98d1f" },
                { "ja", "6a492c0ef19c8ab252c3b2c68489b80b7900129a3ad793d060b7639bea56f3c83df4ee44f46fd697d0a052f4ec0729dde5a014bcd58d912ae3d3ce80e19fbf91" },
                { "ka", "ffb20bdb84db889e4db8b228509c8dcb988c20b72d8efa9f37d5dc541ec8d18fd2f273a94854455fef114f4c0864bcb6d74d0ee48374a1a00a5005f7804a9983" },
                { "kab", "1244f6f9e5af8b89cab6b492658a6341d0d82aca0615c5d5a699209db4e4b19ff33a61981c4480cbc741dd6702def328aaa2f44b13c6b9775030ec54d769ad4c" },
                { "kk", "6542d16b998ac639b36148ce7ce5df000425de28abcdbe7adaf245830d7e179d0afd444b84eb8824e63e1db7ab9a3d5d6ff640f4a38bb963eeed0cbf2bf6b7bf" },
                { "km", "c25f70543084664c540df00b15620976b7914d5e80615bc4942c3446dbacadecf41cfeab9a3463a8012504ceb4ec6fb19135b7d08bea2e25daaa001aa9a652f4" },
                { "kn", "70fd2cbf8c8fac22273202f5a2fe516895f4cd13b0feba3fb9e846bf0581493a1f36b34cb2ddae188dad3119de119742d3d74f07d372d67dbec41a6d501f6db2" },
                { "ko", "1fbae7174a88b2bdc0a63bed13ead594854e989370c4edd4f299eb72789121cc42d19ff21d7df379ac2eb057565dd44ce80c8c295c5fb2218296da737062729a" },
                { "lij", "eefb497194d31975df27c05c5ce13cb24d300b0f8f01ea76b52d215e84f1ab1831eff5500b781b583b67bfc2260f9d21cf42d3ee7a76d135741c0e087ea3c430" },
                { "lt", "623dd0ef7728852bb6a1370be35fa74d6815aa99ddcdbdd723121e9be84ecde5f113b7b25a9ecb3580b62069bc61b3c0fa6de5b436272f639cfa31cd3004601a" },
                { "lv", "69a279b5a3f78190fc1825079c140f101c3317fda7034f0ba87ccec33d42eafea73c9b4d31da466d7d95662efc231df36a152e2ff522fd50342e550f279fb2d4" },
                { "mk", "993c8faff46da81d41c15bc71cd14d19f847286f1d9bfe5b0032e6d25f239b909a6ca629eb25d6f190a7024b044ac4277c64a949e0fbb6334b4a1964c2e0fa34" },
                { "mr", "9f0e31a2b8e2768d010700f9d0c90a9608667f1ce1ac8a09740d9a0f9e909d595769017e60452461ec011f1f10b3b4675884b3a44361ae2a7a57414e0c39f9f7" },
                { "ms", "755ec61a41b20a15d775af6b4eda97a50826ceae16df8341633862023da196520e63eacfbba1b5ed99b5c8e23d72f95f76dcb5a2ac8280e87b4f1aa32c953db9" },
                { "my", "ed6df14702b1546f9b7e3611572116e973518175985ab30f99986d2558268390739980f22623e210383e3afd4bca5c14b796f35491c9299205018ebbb8d90ebe" },
                { "nb-NO", "af89a97c85f6892b51a342193e4b9fdfe8dd8a36f9814ce9915059a8945f4b16c31a686f1d9c9b62d3717dcfc8e9cb7b20304ebbf422f65eff7483de85996300" },
                { "ne-NP", "352e9180e87996db304d3dadc144c0881b387ce77e414c916341b8da64cdd6aba340748e5ab5be456f7784134be5a9afaa8476271d74d39e356215058e319356" },
                { "nl", "4570af59727bf67f58ef4634d629965230f9dcd5e387d1f81bf493e85298e4f1864b867ae2c7c8faabe7e8faac966af17893610ff8d6f47e0c707d8e02e1f2f8" },
                { "nn-NO", "d2797791f64db234f512e395ffe7c100655261cfd2992c24d8e6d66547b4f5f8505b16c32bdd591e8fe3aec36ce12fcc815938decc1efa2aa19534cb0ff79c3e" },
                { "oc", "2b4e64ff4fa13cf12826547b33eeb24ae4d0c7dce0679660e25ca76ba5dea98dd73a87f8fe612baeccef2836d3b4b1eeea41f9e5a291a43bb86423b332f784ff" },
                { "pa-IN", "8588f18ad1bfb7a994b9e04e727f63207c3dfee798b6eb971e7b3478fff8ed528b8b5978100d32a318bc60c5bb84f150e5868f858dddb645f801246419e9938d" },
                { "pl", "a02027162785cc2cdaaace49a1847a4e8ff27f7927ab1f2ed0ce4829a64c4b253c27675488ffbfadbeff23db49e40383dcad016b4d675563061c6dcf8034ba1a" },
                { "pt-BR", "169598db529f54326ebc4b628e31de309385fe2ea061581b5df6b377a7846f48b071312027245b28f7e9ac8e7ea009c77d2507b651c68bbadd2308abcf7e80fa" },
                { "pt-PT", "f78f1e5a69d9e6d15d58ae7e73cddc8313899fccc2f140e0616dc854628bd4974ce216d9a8995a8eaeff026c00027b3f73c19d95adbe81cf1c16fc6ae9ad6f18" },
                { "rm", "9c4838113bc558cc5be813d43d02f891a0f5d073e7932e4b969e5a9b35978d539fb06acbb20fc73f9491f8ef7908f8e2c6d462b54329439c9415417f1bc13763" },
                { "ro", "e054dd280ed7bf59999778eb06e7d44a555b237b7d861c5db1cdd8ba5b966ac5add560569f171e699278886fe38c3eaa2b7ac2bcf36ec211b60c2e5db150d6d0" },
                { "ru", "9ce976d2090596c0065d014689ebc7d033016ae63b1b6218688fe6470bf8c11b20301f93c42a568374eab13eac03a0345e63e43e85150a2913e45df1ff208e0f" },
                { "sat", "1bb7ef9a13cf531b858cf494010d10ea1e476f22b2c6570fa75b61529d7f37ed7dad8f37e0c8fe5884023226a8ff1d92c6b2fd880c6183e8658cfef958cbd30a" },
                { "sc", "84712b047a5fa600c1a8fe92284980f17655615e0cd76e579a7fc9353311fa69515b74951a5d9a86e2aad3a87169643f99566ae94857ec8190d05b125402c6cc" },
                { "sco", "18c736864ab19dfe90d8487159ac193df0408d13c22c7a85a2db86a1a4f212e27f916f9a4cccd8240df2de0284572e01c21db83c8e126a06b79f40092c232e7b" },
                { "si", "bd7b833b2165d4fc2afb5d593c2ce73106250256e7f1b4a18a83bcf34db1e7164ae48e6c5aac4f571c1d855646cd3f4db3700a515e8cbb97ef5dbb1f81a73c3c" },
                { "sk", "17b17fa3e52bec8592c4e8ebdb69bd8993930f1b30d9d9505ec45d08cc76a5f80b040f42663011601623024d14cdd48d8fcdb0754bd1ca016d5943f95cecf15e" },
                { "skr", "17e06082ec1f90c0d98de15d39258ace1a82fcf5602ccb57bacaf1a9c09b3b285c86cee27df77ce635cb7537d1f5f6c4b83b6164be0d33149077c0f27056db2d" },
                { "sl", "4bbcc141003b45926eaf3d05f4e65eb4bfb1c69dd6a5436d9019a89b0faa6c467c5c671d08907ccf46185382605ab8d743b25266201179fa2264d04d97a7b642" },
                { "son", "360e0702ec0efffe62e136580e683dbf9d432da7c9c1fd92e3623643ec84e7af45cddd14d61d824abafceafc0202d0b4af0f73acb5d16c64cf965cfe1175aa68" },
                { "sq", "464df3cd61f88bc79b1fb53c969bb75b5ee10fc0e9783f73e8ae815046138ed6d688e389ee759dfdb69fedf166934cc526eeca2a6e7acf09fa45ba46a97f79e5" },
                { "sr", "96e085950fd35a860d6dfa343cd1a0875ba1a7e98de845783340b884d7174749b9e32c382d8641d51049e72936599b6c0768b3cfc1417ccdf36215745cf13250" },
                { "sv-SE", "8b1118f61413f75b7e904caa6dfa969554c27f0455fb5216524c0eca05f7da47d51a878d04779a5e90a6867351a751312e05b879b181cbadd60398b4844a35a4" },
                { "szl", "5d897e1d93130cea2a32ab117904a5c457e315f9e8d2fef12689fee968e0ed8a73a6722c80b69ed19eedb676545e64e327bafde5a7aa3c99fbe53e04eb515c57" },
                { "ta", "ffff50962edc4b7b7e363eac5143ce903575f1747b2ec93d81ead1047876612ba75da38aeba37f02ef91fb9a249d03d52135ed3bb9d6f31645460be19d215954" },
                { "te", "b9a348274f3313c939510269afa7ba5dd2484fa436735ab1512b8b2cf60b9536b98badf0e1515b595c79a4d342bbf21bf20803d14c9ab88414da88984aaacd5c" },
                { "tg", "9f062d53a6212265a7e9636f47b4485bc3ffd08a46ec2169cd0ba6a9681fc2e89b760c96193eeedb3329235b25dddb1bfdb3212a9635f38c51a15b6ab8a47dcf" },
                { "th", "bf2e30d3746101c42ea2bb9672ec06710f78e64aef05a3f9c25724bccd2cb85e5b5f21ca1a5bb112120928fff1caec4d2e3f76033d2368533d4c0d7f91bf7654" },
                { "tl", "57472eb8345b40ed8e172c2d96378dc59b8555e224013a40a1b00bb7fb97fb49e5cd5bfb3ea782c2cec6d3cb7ac1c67fd993f7ef5b23b60a4dda5b3f694565ae" },
                { "tr", "8b41aceece754a76c941c239a7b42b2b3880a33b3105d5a485ac24ddd0208195293e19aa9e808e1f6d8aa40a8662f8b2edf9c93e147e2725daf5848f55500ac4" },
                { "trs", "5368e47a33165bb1bc2cb11aa37862dd9c9121b48fc4b84efe9e111d717ffc7e24aa40d2fb78e530f53cff171a51c2b9bab22bc990182e731f4cceefd157ca93" },
                { "uk", "cfa6cd9872fd4a618a4daf4483f82c1f6694143d7a6fac39dbe83fb71d06d1d045743196494a03a74b7a7ea7de62838a0fad197d4d3eea951f03f6ccebeb3ff1" },
                { "ur", "fb1c4a97051fc4c48c1e0646c7a3f7cb7e16558972397d2c5936c9b34fa2141b9c3ab7680d2fd60e01f36928d0558159f30fc2ec618832c41f9a4ec15990c5bb" },
                { "uz", "c781c86a201abfea51e391d3718a7733f39cf43d8fbec12a4dc80c2338dedffd38f4915dcfd024b4ef9c6abf7ab3fe2687835f419c01cd9f1e8f772fa8a1d99d" },
                { "vi", "4c9feef28eb79b1b1352196865912ff460cbf5551e745cd3a3b1591243958296491fd75b4a3c1361e0e072d8febc62957ab50eee08657f3ab3d7c3e055d8f408" },
                { "xh", "1599b229a23501d3320321d2525fe2296debc44bc49e2ce528eb522ee58f8fd103dd27c61b5ff6fb6e7c02bb564982fa36884c63ba9be865409ffb494177f779" },
                { "zh-CN", "2b67b58a3cb7c5276a156e12749fecfc155f0a38c7b79fba6abaeea739c58b7a6a1c5e370fe3884dd18e82c9c046c982ce670668752cf0861a121a8c9a733a9e" },
                { "zh-TW", "096041b81f2a8898ed9ea553896bb02aa7420e92c0d45b36415d14b80bfed8936a92e16863571ae071e6bc561f8bb8bc78c80a3f38414e30be31c41d8ab1cb52" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/128.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "c262ab7dd8694f9c6cf415f94eba8aa5a0530c4760b7a3bccd75185bc0e1d326dad6682e1ba3cf2c87c0a01432334de7f889f10ad5e1b63662895b89fbf7d0ec" },
                { "af", "cd95962b31d366fffaf3b8a3a418ee2d1669cbd92bcddbfe3276d9c1a1c013ad871655e8790c9e7fce82db5cb344ffd5e2ca16f9ec63da7eeb733b659a363585" },
                { "an", "2f290bdf16c46bfa48ad03dbed53467212f798cbf8ef750420c4f183f52d445090ca416e4fdca704a39446c527fa9c1ce1c44e40e184155fadb2a7833f4beb04" },
                { "ar", "d2d377e4ebcd526aee9d299c5d1179815959155a82218389f6e7716d8d4536070aead1f42ca271a61657b092995b322547d451a7734fc5a9b1cd33cc54e342e2" },
                { "ast", "b6fffcbff54be68571638ea1ab326e338d2faef16ca61ab4ba159aa02970a3ae981a3165427302f901d7ca19080e7068748ada865306364eea8a28ea1d0c2c94" },
                { "az", "a62368277454926797f2f919230424c84b293f1bfbc10243aa6f8c7940d8d1ea9134a6e5fe8700f219b67781d4b0a5bab6305cfc9c567169cbabcdad6aa3adf5" },
                { "be", "d6525c5082395940ed3c1de939564c6523ce6849b1d3f2fa4104cc4a8f6793b69fe0537afedb410ec9b027420dbd04a81cf1f136d41e4a616f6a3a6b4b132284" },
                { "bg", "a1e5f108cf4a12467b72f3d945a208f63f0aecc8d4ab672336f784c46c80f1a1772366fc3e610874eaccc0626b7d5d39c38266174674b4c5ec2fd9174b8e6a9e" },
                { "bn", "8b1084479047686206e2e607868661a31e8c02ca86ce72e661c7fe0a0ac7896bfaedb28b17099b2d76296ee27c316c935e9db10994239a51f2252dca04342bb4" },
                { "br", "169952a9ae2f0cf0effe0229778c29a9ac2a57f77bca8f30e16ed338a6b2c7bc26a211bbfd4913c5f163ae722b7bbd0fa33a090c5ddf6dd39c1a721fe5018933" },
                { "bs", "ebc9d9f4eb179a554b71c7b9a5d347f40041e53e7fda3a20658d26d7c6eee9349e13b488d6f3feded8ef8b35a7214db8dc366a5977f6bdea55e250b4f804b9b7" },
                { "ca", "4754e93354c5efafb847f7e31c93976880d9542230fbc099a8ed1a5e170cd559c335b6199cc654679fed052d63e56b77ec1884807d8307e760e7bb8f606f2ab1" },
                { "cak", "3d0f0f8aa331d1478db2f56ffe57307a5ae0a41ee01457cd114a4c48ed8014345bb971dc78ce0c1381eaadef1b3b6db9ab8f9cb8ed933bf60e614358429fa28e" },
                { "cs", "12a876fbe806851a3d665527ce955c0cad2ceebcaafe259fcbbf7c0ce11fb0835e7b400790bf80524c35bb829617cd1292193c3e57177a1d5f61a9dc327ed7ac" },
                { "cy", "2411352d2451780d0414db3e9a98c4212fa2fa10e778f7e9c1c7e2cbb51d78e99ab550f6bfa097b3dcdfb12c6292e78697a0961490298dbc274918c620a34f6e" },
                { "da", "0c93d3a07301fca9cdc431a9b81f643809e75abfeb61c23a9b1177ee2136f12cf3d3591e767163c8449efa32b480f55076ce648909c37dc950cc02f0ea8c876d" },
                { "de", "13e067ca42cfe77392884add81f5cd88cfcd5ee3e1f4a5b8d86ed41f714399b6bf79ed2305692cf1b5931674cd39a48d7da8805609910b15c03aca4487b42ce1" },
                { "dsb", "073350ec0e84a78f2c327eba99136897190baadf2c0b6437da6eaddfa8f84503332c6f0aff95c81d46dedbf56085e2ca4e1afa71d0be647c8bb99403c19a8b1d" },
                { "el", "f1a40b5d6c58f77e69857b4ddf52d0d6cefb7e714bf88a78ca44a1c5fa210f7e00d6a90325c9e8e9d960bb0c890624f7b9ef59ecbd20088207083429fc3fc3a5" },
                { "en-CA", "785aef5c6ab6d9c7cb3019cc50d74cefab97d6d4bbad8b094aaeae42491e821ec117160bd5c60ac58607166801dc1b553bb6ac0de9ccfcb16f74ea05db672109" },
                { "en-GB", "2f3f0dff132edb21afabb56e114ec982048cb661e4e3e55db6593bb04ac14d4cff34d2ed2bedba992e4bc8725d3b1e54f120a8318b0c210b80db0725c6a81064" },
                { "en-US", "43d9d9753633f3a42a865a2c77e1597f0cb58304cc2418246e06e2eafd8db6b8b2f8a1b277bfbba6598c3e53b9dac1f564d134d490728904bfe93ebcdd89bc03" },
                { "eo", "b39fd6c786bca30f6b730b14fc22ccd74e820bd0eff1a6b6d91cc1e5b673d98805e0fa1ce64e7dcbfd4579782ff17c4310764a906fcf3b1d8136eb6092c7ff55" },
                { "es-AR", "88baecd9959ed3c2b0c039d2185001f8401bf935b8e67005f7e23be4b8b50c54798a473916a242a7158843c263de5dd885118c79573aa083ea724d0658539bff" },
                { "es-CL", "9e12d6b499e4e65705d66c600908fd6fb1bb321ae3743eaf3830aa55fc874bf2b02ad84a6668c8619a4df9d4ccdc9cee81d90338cf8fceeaed3601259d1e2e23" },
                { "es-ES", "a456275591f573ad22a9226ce7baf4be73d0646b5828fcdb498c68d6bed1b55275aafa76b7e72a5c046bc8fd61ac7174bcb7ef3fbd118f8ce47c1eace62650ca" },
                { "es-MX", "beb15adf1c1cbb80c92481c37175e47e867d510c151d24810f7eaa9d9a33a20a43e2c0ca0b2ff5b345ca90c35aef84cd6e58120a33ce0ad5ef487ccb9dea781b" },
                { "et", "b728ab622b40e655a7d0f8ffe7624d5f54260ba1c097bd7b47fd537df3125d946da93fd000faf7bb20d651aa3912f4622b7deae9f069a41b6365f531a81edbe7" },
                { "eu", "c54d36747ea74bd3febc5a9d41f74dd127041c893638f1ae6e3209075feaee5c6fede9ec503bebfbb1e4004fa7ccfb2120f55bcce0e68ed9882e343b882152d6" },
                { "fa", "7687c071418bf80be888333bd76f3182f1dea25b71480bbabeb1bc12b0a1350f9afd93afdd79309f93da248c8aed8019c5abd0a66749300861ebe763c478ed85" },
                { "ff", "41b63e043c8c0624a68b6de6f0d1f7bd91ff104b4e4052fa7a1634db9b4f02275ca11ff0e618e1bd2bedfdff437430442b34aec6b5db84454ac204fe7b729ac9" },
                { "fi", "0489642a65a55aa9f5ce49757c479abeb51890bac453064ca2b706d9c4562b6246bddf984b1a29628bf4b8084e0fc712b51ce19b1c2d6455d23d68c5e7170edb" },
                { "fr", "617811b2dfa65df77c9522c8584d800fd3954909b82761e69bdc1a36c9a085794ce25de1545eef3d535583822fdeda6b0d84c48147cbdbfd9a26e44dc7765aa0" },
                { "fur", "4ed4143c589e7744da7e64852c2843489af753a327ea9d0c40ab42cc074a35717996067621fd24e522fd83468ee625fe4a300dc8a2664fbc8ad07647862d49fa" },
                { "fy-NL", "e8da0f4dae4225812b24ce75bf75739dc69f553376a77d7ef99fd2591c8a171f9edc0bebe15b7dddfa40c36191ad10e2ce0a2d8858bed436d75533b262d1956c" },
                { "ga-IE", "a6f6141081c9b3329b136461c9cd5ea8931df3db5e99cc096399cde9ae330982c2bcd28dabb6a339ab08929517d14f190a4d5a479b71d416d1ca2a1ce343898a" },
                { "gd", "0994b4f76ddc41a09a8ae5e9ea707672e80b2784db917eb6c4900f6133596fcb1fed5f45349763c0a2cfabf3bf9dabd8ca6c8c1d24ddf049e662bd2d3ae48c1f" },
                { "gl", "11ebd3ba379ed895fb4825feb56bcbe2d3a2326da77fcb0efcb76b6a23c4070f5def1944a0344d92274f22dea230e562eef6f0ed710280b34d45d2f0d1a8e831" },
                { "gn", "aea90c8416354209c3c3eb55896e2a579bfce60cf0b8b97f4585b93e71fbfe6f52cca1dc1c16cca485614902f22a78c82400c2c0adff8f3cad7cc43d6ac65d0e" },
                { "gu-IN", "0449c4ee889e96161bc86df8d58d85b93d2735e069e10f12d8508a01ac87e7bf9eb071da9d2d7256abfd4f42d86226ec795400fd8995e50da177429667c99fbd" },
                { "he", "90761d4d418af5321135b555d197e208878c10dddb2fdd8c6c62e423ac6adaa3693035ab4945d2754a9ca62812e98d45592b949e9fadefe10dd4d5302117c689" },
                { "hi-IN", "bd04ca0aff7152b8e8967736bc089c5db7961b3530ea72a71922807af8df08d846f2827d7fc41b9ee80c056873c97cc284f1dc7826c2768f6a7c8c7d22e8f0b1" },
                { "hr", "46351aff126f003caac8d2f4cf8b34fed509387a18690cc6ad39855ae271b9df161cf433776963e3cfcdd72f8b2cbdbfa72be7521ebefcac3d77181863f62bc7" },
                { "hsb", "48600303fbc14beb6408426053c074668a9280b324ff709d5c56c5c3683b58cb8a1f5581bbc1c12c55e66c5603f36e962f99a260e66c8f1d4e89ffda23e5edef" },
                { "hu", "59e81d82627e8076d92e27ea667c14fcf06f426bd970691cc5885ae431bbaf8e0567b1717ee64da8e351105767371a60ebb8d8acafeff1cff0747f3073cf8f79" },
                { "hy-AM", "faa67e37f0bee09e9602ca88b5abc7146728ab3a2632f2426f44082ae94fac7b539d08d2a4165d494b62157ab7b4f7ea60bd9ee8b62ab3ba12d0723dc8a6ae5c" },
                { "ia", "ab20aad15f5b8117effac13dd077876325066c40c7457e86706422a1d9c0183bdbd5f3814a815dda72db222804ecc9912cc4ac1ed731538d1e7974803bd991be" },
                { "id", "3fa7e79ae016e97ebc6bc3fc4448fad8efb553fd576d9540308bac8017031a5b7cac480b0e69e04ca0ed882a99d46e7f3468f038ece3b07577b8cf3c07d1ffac" },
                { "is", "581ae0b6b9738875444221b0e3d9f0799d7e3b84f9c7c17e28ddbd5055f6cfb6830d14dfa4d23d6c1cc343f7d6a98f943042c15f851d44b18c4e4f5fbff4abef" },
                { "it", "8af0922da8ae27f82c3db0208c7b1833caa5d20a9b4a7e4d0466dfcc45f631cb3efac209674bc1841a4ef34f82b79c5df572959f6c5ce8242aec981cbe54d7dc" },
                { "ja", "f80e460a1b6a0779bd1b0943c6ccc3bbcc65b69df9c58093c1fc3cc7828074e1c23f0a190483c77a912e793682c7d4b9d5f559654c96d1ab3c84d3508c58d5fd" },
                { "ka", "ebf32e09c5a2e8224017034578b2d12c1e2ebb2abb41491bfaf7d30db9397de4e47218d3a38c246faf91856901e6050994e66fbcccc5cbfe7f23efc734e5e0b7" },
                { "kab", "380e64b6dd3b4e4d9ce07b477fb50a3ae39d771926015ac9553f3b72b71ef8257ae63dc3f3cee180e765d78bc7bbc0f81374a0b22c1ea555d77205c5ecdeaa38" },
                { "kk", "88b36c75625ae6a794a503afa58f5b6a904ecaf077981063f7db3c5e5d252637ffc34ef9f033f94eb2b810ac0576e25fd9157845d3bf72ffd13364556b7bd4be" },
                { "km", "edcef42389dd7866c79de5c25166c99776105be8947a7e3316d28daa4a8d33f2a1a2ab60d0c3704fd2b8fc544fc3e21963ea345b81cd0ae9621d6fd945a99133" },
                { "kn", "eb11a9abd63772e3e33a81d590f7ba079c9e01245236137001e02423793d4e3299ac96ceee8131db978e9f10f4bbffe1f55f589d96d5b178b817bf8890b439ee" },
                { "ko", "cf4fbba808035fa882df6aa6c92413090e7435023470b6301514a74e5ccfe23a4cec5cdf6a3d521d6302c74b4fb997b94772fbe9d452984226d18f226f1cf913" },
                { "lij", "fae2f97d67c9c99406bdcf69b38cf56644a0ed030222f20fa940066ac023bc4522f2ff9f7c4208097b8bbfed2e59efc1f969c72a8e9ac59fed1722284fe94930" },
                { "lt", "e1505042818f34480e1ec654a43e77753d7bf986f74c7e00da06792071b2e4dbd1b8ab6698f03a47e0f2e49cd857cb70b56c44fabee68c329b9dae860e0f62f8" },
                { "lv", "3a8674b9262679d4fb0ac8f30017d3c2352e229106a6e68a558b9aac2dc1bc23b226b3d09080e978c68d5016fa697826d8346f00ed20ba8c983b655abba82cd6" },
                { "mk", "178cf0a0574c692378cb4b1de132fdc95f1250a48d74b06ab02fccfaa4f239eade1945856a54153d05de4e5ccd16259424ea0b32cf47e0173ba24b7b59ada052" },
                { "mr", "ee12d1d5441827adec7b9c2c57ccb4399c77f7e9c45738cfe2388bc1ac0249ba3da619eb11311d2d1b155f969435ee2eb894f8fbc134497ea38f700c9ea6ff7a" },
                { "ms", "a3c263bedb5907ed325c03b712d5c61b6af262206402e585459d865152ba54912ff7cdf503a1f9d1dec0b497c23bdda9ad09770c54d7aca661015fc2b027ea96" },
                { "my", "3fdd991c8ef03173bfbb5ce22a849d78251903c9aef17a1a494e89d1fe22476827518071c9b00a8cf21c856644ac761ee80911f36e56dde5049c5fc7290943cf" },
                { "nb-NO", "7d265edcffbe45e7d15243482d7d29691966f0381fa75c5a511c3a4ad0f6e918d03b9c82ce9079b4f3db6f974d0b1ce6f1fa9970c016b4610ffe010d7a40de1a" },
                { "ne-NP", "ee10f3ed1796caac418331c0ed2b4205ce1a3e1cfae0d40cb4b29a588a18776bdb9db8db7b2abab4bd9f744322928d50ae7baeb2750aba21e42a36fab7b65a9e" },
                { "nl", "ffbd84c54e5717a487c1551cca0a7d705d3169de1ea4fd1506b493f5e663198899d45ffce6fd9e3044029b26e3b63ceb2d298cf879afea88b9aefb519e424fcc" },
                { "nn-NO", "9c11b06e7126e51fdab70afd46408b6e536ac6aebdcb4324bf87a7d5fba9783ef6a15a222844fef57322e70f4de2edeae33cc16e069dc45d21f2661af764ce55" },
                { "oc", "76c8bcb19db3ef011d198603d1f780ed22eeb74dd31463c5d0202f0396a831d31095f7c25bebf239c2965340bed3df7b945fed2ec290b4619ca6bf95030ac706" },
                { "pa-IN", "cc4bde1318fcee1a583608415a56d992c10229386f5767a053c950b7216e6672d2d6552bbc29eda86e386d16d7a9c4c6f62fa81eb2d252a62b06439113be841c" },
                { "pl", "caaf237818311dc14ffc0708ff247a1a4bcfa7e772fc32ae1f2b6178b80ad92b9fec7be2913fa3fb091000d2c877954c6894de65139378575f91df137f96fd0d" },
                { "pt-BR", "12b0ef8f2aafa44fe3d3515e507b9e17dfc44108e8ce1a6a882e076cb2a410cd4f52ad5ea5389b6c672209eccc223f27fdc9683b48ccaffae635c81f06c1fd27" },
                { "pt-PT", "da19dde7a45b574807e7fc0b9564444e2c1b27af1854676b8b43ecdb257581c5defd39ff61d3acf545a168aebf24dbfbb85ff7376da0836c8b34c906c9103a9e" },
                { "rm", "11a3038d67678b96628fb8d4d102e4c4fbb49547f793efe5d3b6945d553bfe9c89654895894f7d7a76f084f6ee2585fe856769a9055ff20092fc8ad4b846a135" },
                { "ro", "1220bb0f96d680f657492d65e1bcf92d301fa8025b88e9111a29dc62a7f71a25e5e0e17b1a2e6c67e8184786f6826779f335044402c375b3367d67d1eebb0308" },
                { "ru", "24aeb994369314f861bf483e6d715f0f54b0c1ef417efe9ed32d3b2d4571a28e9103e93f4f920658abe548140229e343ae93ebe20f13c5e57ccb15c0bbacc195" },
                { "sat", "a13d271002c3fc81a7ffb55872c19be21239e41bfc1d6e54732f58760f34843e681f468d2600d65090518fa9e4690a93765f6bee297f0bce77c2c64697f6a625" },
                { "sc", "829cb441d8245fb4e07ab50698a8058e4ec8b75f661056dac1307eeaad6bd55ffe4a5b85997b2208d43c6555a7ad0b90516ebe3b5b764828fa36f20e1b79a678" },
                { "sco", "3ac180350aeb60328feff69fc54dd6cc33213e0a87f0d3920e3ecbb40ba0c3e3a44b1b04ea4f0ca8e0735b7738f7e112cbad7e4df0ef5bc59c861631f16cc6e1" },
                { "si", "bdc7659568128841e9fd13cff2a05dc3aa54fefae6cf4716a0db3daf27cf0e81f3028f27e91b444785ff4831a4fefa23a8e612ed7e32bfa58e88caa34e0c0aef" },
                { "sk", "0fabeea06ad3907c5d316f9d4aa4c6e210a9d3b464af078b59d0f99e4cf978d68e8fa00dd3df8bc31e0e02570a69a6fb690f884b87bf21252b70887f1d38c879" },
                { "skr", "4d9c3c3e17fe7330723d113657d39de3ac6fe43800a72a4e19c70c9de91321acd61a56416d6afff830b9f3bb1fd47c97a7138a8b1ed82c204a424ede4f2dbbba" },
                { "sl", "6f7d5ed1dc78a6a30bf1b6f61d8dd63f6d43ad64f21f9f26cce5973cbd89728382ee84fe6af349015df5f558c4fb0c4126fe875a79962ea4b88941a913945000" },
                { "son", "78dd8e7fa7e12bb6cc612ae474efcd4ec6323b7322d7a06a0dfd92a76d7f2235f6f1debc7e8d838d27ec5c4ea2266031bcfaad64a5fbf050692cd7ce415ce1e2" },
                { "sq", "712afa33e5c2de7596f1e1d4b7171efc11585df749db29108e1476bc4f7593dc49587bafdae63a49479ba86ac991ffd1c38091e27922360d0d7b37b179766de4" },
                { "sr", "9ef48fe5355fddac393dae008effb7ef9b5e2baeaf039c0c80e3d52befb9aba50bd6d5fc107be09505e2b3e49ede2299b0ac90cdf4d63f3b89835effa7985476" },
                { "sv-SE", "8cfd8bbf559fd946f0cfc809ed1eedc2d13d46863a751cc4a91b37435f1f16dc1d7295789a6259878d934feb0efbecbd720ab158dd67bfecd866d86652a67f40" },
                { "szl", "c832f940fa83288e35f742207332d239af1afe48124cfb371372878881d40e639eefb3e647c16e9ba31597089e3d5a88b2d30fd2409b898e994d98689a4fa68b" },
                { "ta", "f5a4a38e75ec8cd3181f8fd66124c6d8776579a8aa3d4078cc3068c6ef99341d07320fe7aa29554e83f466e761b0632a7f2db9dc40e27e623e670ede4fc659a2" },
                { "te", "4364808485b80907eb950e6976474ff1a37d7a154af88c96738d2993964eeb707fd82f750922f4846a54162797c707b4ea8487951dbdef6efb70abed0c43b7fb" },
                { "tg", "15d135357f575153087f06fbfa10e95fa800038a9e51406e5a03fa9e631e23f2f7cabcb8ded564cfaba9b176becb688c89564c2362e8f080c75e3b3daea03249" },
                { "th", "62c299398b393893c9d406e5bf132472f6178a5d9a6ba3f91be400727b857354ec8fad3c31262c7d4f41bd20ec2c85d890aff4a8a450f835533e2a0cff3049c1" },
                { "tl", "476f194ad191fae4aea821d824648d8f3654ebd2322cde9bd12a0f0c5e842963a5f6253bb89f9ac586eff8756d864484dadbfb0dcf7faa40e29079e448d6821d" },
                { "tr", "c16586c7dc1b4300da64a9ac37ae4f532f1f0b663b8ee930303f0d83ab4ae6f8e5c54c23f51713e224539f749600380e9b89b55867e6535e418c84bf99ef6dad" },
                { "trs", "cde50cf5bbf57ec359d15f83327a8a792114b9a4723df8208cbc1d2c73d622868b6ed4adaa9d6c830bf06c510b032624c73d544b55653a33b7bd36b792c07e60" },
                { "uk", "2357f74c18fc5a02ed9e2ae86fc475e1ab082044719251faff90b312dacf63bbd29b11a177e335831504fca510e5aead4961b763fb6fbfd9ced044aeb99590dd" },
                { "ur", "7b8cfb0f37887c395f81645d7471650f537f7ccf6ca0823f1018596155b323d8a1798af97885771bed6cd724f93cf80c7982cfaa04f1e4521c53fe29c52403bb" },
                { "uz", "4a75092a142ca963f200c6fae29796c5efc339206ce238a929574fb34b517be15ec3a1bc6ec81f737c8a71bf7cdcf8fda9b7ec518d132e9954e27fe74d02cddc" },
                { "vi", "a4931db911742d98cef48323789c9bc660e1ef76382c0d89411c998e9a4cca5da8a73b0a42b3385da9ac6115ea4a08ddfa99bb46267bf9d180e4c107de994fa8" },
                { "xh", "0a127ab28d13de7a1f8cf8a35fafb1da53cd6b0ca3c49a8ca9f78d6eb60e9e57ec94caec4f6e00caa98d1026dc633bc78cfa1a9750ca3800d5d5cddc81b1491f" },
                { "zh-CN", "d096cb88fb7a3566f383e2e3c8f3f4102c85c0e9e6e9ec25346132114202d1b20cbc8351aaf786d4e9cb7aeda6209cd2f9fc22c582ffa6a039c67d68d7e86d93" },
                { "zh-TW", "ad22889a052421c82ced68baa3d735a44e37bbccf099bc81dd013c0d59f88a383ef953fd1db7668a44b15ea5d6b8a23ff9ea7b856a363b9839ca897e35f691f5" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "128.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
