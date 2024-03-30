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
        private const string currentVersion = "125.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6d833d0d775c31bb92dc7538db8615b566c5c07997594f9a16b7ea418e4995ed525d1279d2e82abab43fc121ce8f1c116ce37f016f9feac0035f3cad265f784f" },
                { "af", "5b90c7c8126fc9eccc7cf2171732b8fead8b1e552f68dec12c21c8c078bb6810aedc7510e1131f1b719e6d7838c54f19eb6ffb36a843bfe1501ab823cb8053fe" },
                { "an", "392557775467ad049c7e4f5c4a15b2a341d68f6fb2d4f2cc7912d4024efff125a4a05d061fd1dbe162a6c416f056c9a3a2a07223571c06488944d588cf7cb5d5" },
                { "ar", "c767ea48b5adde7be2a52724826c462d73c2462efecdec61ed368341fc4d9fe740757485b699a4c3e3d2790f0fd879b9c28676b32342775560034332a5519e63" },
                { "ast", "48fc403e048b35cecf88eb49d497dd35524cc6c38b47b01d33b04ceaab1861576d2c094a6f807f2431d5939728a81e93a08b47a595a90e4e8f6c04bc9a6a42ba" },
                { "az", "916afdc1fec1a509d9d66a596057e6b4f84341355df3ef1b6526ff80742e0bdf2a6e7f2d2aebc0bf2c967ede859e777e099232cc09d8b792ea3287285ca5a37c" },
                { "be", "80a6297b6f249150852ebb54e182a736e7976975d0257c1631247aef4447df237c64788e0af34980092395d6d4176e0b948fdc1a50889ba848a1b2158bf9863a" },
                { "bg", "087daecff2fa711d12d65c09e4905e4d3d0fd8adccd9672ec17d981ac9d1ffc4b1292a4648ad4f775ed42389014c86a90c709020e5e06392d20a35659cffcfd9" },
                { "bn", "fc18f5dc9c8eec70fc592dbaa3b7006897a473343213d42066f06f5100856ad5b8cf59432956cf231a455196aec5c7a1d4765743bcc6d1b9416dbc2631dbd64f" },
                { "br", "105d9ef8c5c0952eb424c09eca77107ec8393022a8910bd754010e91472865fd483084f3f63747ac9147370ec042ace061d0d3ed69d53db14277c7693d0484af" },
                { "bs", "5458ec8c7bb204512aa2577b5f2f0be249bb767b81af58e9625ed2515ab3c6a30ada834f8c5d1834f5faf34382d200513d6f995055861fc264bd115667da09a8" },
                { "ca", "94c0751c00e3c1e81af32546decc71bd393c3e0e3339c44c15e0a39a3975a2ec4b1b10d1c8dfa64348286c9305bfe2dbc8ef24543b840ee1ba415f0bd76a6781" },
                { "cak", "c94ed5b3c5aee67003448e8123b7ab9511d894eaa1404c6a18040bd74df27e2d4e73f47c42464b9b61ae45db7154d070c8d50183a2486549b202f418d2a75fd2" },
                { "cs", "2dddaf2eae01178a3e6fe316785a80125ad6e98195090ccc7e9cc020f4a07e41a6a516215808d99b8cce323701ab60fd1e2f1e78cfb981de2fd0294442fa7e11" },
                { "cy", "f05c70fdd4d5ff399bdf3c07ac3be065a6d3a7fa9e83537a4ba7001fff7336702b59301899bd875f1168755432c8faf829d265c10d1b43329d1d82073a2a091d" },
                { "da", "71caf5e6af14a0925a217c6ec4a9de6635c457dc0b74c40bda0c10790d12e3a8f8c7f56dd2650506b187a48ce7ba4f98cce6493c4e449ab53df1151d43f6c6be" },
                { "de", "0afc388e60167907a93ba416ceef33bb9bdcf3f15d2fb21e4f274872587ed66da471929a9d26fb178371176985b76108895d15c923d593b3041eac6fde040fbe" },
                { "dsb", "30396394f719f0c6433966521a365e3f1a42d42eb1b25dfd44f9f546f15ae8bf2ea952badf80b939063af6a785b4cd6aecb7fbb214258dcab9853f4830091ce6" },
                { "el", "c41287bd1e43f2180aa6f4b68afb2fdfdcc3de953c1b6093e413f7693654d6c4d50430df7f99bcf4548cc4b0afcdc3598e6bfd21e10b94b4e4b30f0525667e1f" },
                { "en-CA", "3f2931b04df1265339bd07bab83f1cd692bd0a68b389b3ab2a76d74a9b601af119daf6638bbaa84bd6cdf3d5c2213611a9522ded646cb9de65d8f376c88b334d" },
                { "en-GB", "7d39156641c6fae95efa7e8f3f66e502dfbf61fca554f15412ac40185e351658b5f41d0d7e0b039e789d814575a307c77e0159eeb72557b55b87ea7542255559" },
                { "en-US", "cd14d14ca11d50e3e37d3944de5cff99d71fe2cf719cffef2c5769a28082069ecdd2c69c2d7afee0fb99b2fcf7e0d50a7e2423fca550ae10c6094728c6e9c91a" },
                { "eo", "39b089a7dd95a6fe0aed83c1ed8fffd0441b3818422bab8127ac62361b84a21310cc0ad7d02259d72afbee4f3653b280255fc34ab54dae16e9b5a87927924df9" },
                { "es-AR", "a1d4947c35906e56266d1ba53ba77e6a14d0b9bd56a399922c637b91c09fad32386dea90a25cc1df52ae39f818f0668e9831ea1e8b51591e206c400ef44d3ca7" },
                { "es-CL", "81bc2edcacf470862a36ab2c5bb2e177c645d29fc968072c3ce3a99f70185bbbe0576cfd68d8bc71f8400b008671876ce52a7e0e8180f257049c0d228ae2ee75" },
                { "es-ES", "15849a0469c7c9f2ddaa529656fc35f22a6e6b9fa43d36597b5d5c0096984f2e05857256ed0dedd0c4adc705b3309fe5f525702325ba9a882afb41165a1263f9" },
                { "es-MX", "f13fdf95e9703713b6437c4fc17e7f39c2ace2693a191b75542c4854c1c4a9a15103da7ced0da19c62317b5b678f6bcda4ba9ffefa49acb7a1cf4a1e3088cb43" },
                { "et", "1285e19cfd13c7832f17f76fa607ccc92b4eccd2023fcea257f3cb9555060fbfc842fddddab595be429fdab45ffe651fc546ffdd259f7c242a76f751fcf4241e" },
                { "eu", "f6253346d742a97c41b1640d1799d956d644002dd24f2bfe4271706b01ae0f04f6614c1bdffbc05fcc875f1c0d4a8dec285c024b55654dcb62516ef08bc83d66" },
                { "fa", "f072938a3e7fba3a79c1cee217517a54f71e83b1fa249e4995cb97a5f449662bdee5874b071da203cbe542125a73869a88c6457a1066eb997bcb711abc0ef8fe" },
                { "ff", "525af129b87db0bb87394c3fc458b3b35079e61a0292f358735a5ee40f4a1d4c6c56e7a76fd77148df98fe8c1c662d675ed6d52c8ba4d5972d1cbfbedad0f3c3" },
                { "fi", "8563cb17af80bfda579d5313292c6d3425b0c1544daac609b3504541eeb9706396aff93474d1a107fe62a2ffb236544a094aff7e4a0a306f5269e1a8725070b3" },
                { "fr", "61780a994bc597c3c39ef3e738061a25f92ef9680060861210fd4c8cf1f396b3fcba3eb58f066e34ef34aeab9096b7dd3e7070a012410437bf1e71c7c4941022" },
                { "fur", "d4a7b62985b93a3eb90c34d2e55225cecb08a9c931b2d0b97c564e9b2d00b161ded7e1d8cc475065744e780dec370a3c60cdbff342cc0d419b5c7bf0ce364217" },
                { "fy-NL", "1d226181ccadad9fa4f5684f73f4d19d4b66f6c642d622312a7b748e13363f3d5157f5d088dbdcf8b9a21062889ebbd332c19d28c12088f1bae4c2195f73e1b8" },
                { "ga-IE", "14f375e06d24407a48b2741468ee2d9adaf33ffc0d73b69bdb9c53b84db691a6b0fa9150ba83478af442cad32a61420595a5a18fe3bf24d431846d0196429382" },
                { "gd", "6a2169407eba617cf66605394b91eb46b32261060c614736f98172904fa1afcb75ad6091bd131827ae81ffea540fc1499f510e0aad552fa868484bb7aea9f4f2" },
                { "gl", "555ee696f2fb0472e016c7160766f050673d6fdef4d6621946e7c84392a4f4908cb41fb944f8fe99490a9044904bd107df8591e2ea67f48a40bfd27ec294bba1" },
                { "gn", "44773a4e0dbbece7aece7f9ab8adf4811e45d3b89f5ac6f6a35dbae1ea844bc9256d0a524565a27a16a7b5e9ce636310ecb89dd7649dd105601061663b1b76d9" },
                { "gu-IN", "9ae3f0b68759359980d6e21108610d86bce2e839b7b7a3e6f88b1c7bdc9e2f8165ec4ae66b35e82d80db514d4abc099a96d2b96e2266eaf5f0b2b6962763fef6" },
                { "he", "41f9eb5c656f082b24844ed60ab4d1194567a3ca222440d0581f930a9265b5af96dfd9caab569e2b2759fd3221f949bc7f68a5afdaef45d9fe836369d8480624" },
                { "hi-IN", "1bf7c3397be63af046daac690ff5379ee6e75d54d4dc3b2e6f8e2e84aa79f6100319d065a08a5c43368e7a2caf7cfbf21edee134424fa7a0673d7063a67fb274" },
                { "hr", "f314cc99b6bc34173c05cc10c8bf72029e2ad9a6dc473a39a39d5942fe7751e2a2bfa672c7fa5212b6faddd4db184396de6e53bd9e6a653dc10f18f21d80c4ef" },
                { "hsb", "fa19558c96c39b1599fc2fd27466fbc1d55af1eb882f03a5a7e0c9bad4f855a20a26eba029e454e62eb7fa6e5dc14dc9dd7d1437f097a40e2540e33b15ace3e3" },
                { "hu", "01373d13acad80e1b28ab5935ad335fde09e9be4ceeef158d22066e6110d92813cfa1391d5669ca47c1c30582052af128d74bd74c0d70b7402954b1daa0bb7bb" },
                { "hy-AM", "57d471d2a7d54e08a7c6315fc2de4dcfbb6cfc36835809763355761f9ec95bece0944a8f0d3be01e3f48b4b5e2340f115bc86c3a9f1d6a4a754e8565a9c36663" },
                { "ia", "67744fc6babd9954acbe49aa2392ffe1fc50b68115b30d903427681dba47f7dce58b69b4935a9463a6e45d45f8e4acf88427d4febdeacbd071d848f14373924a" },
                { "id", "31104fdd2358a9466ff057420f670622c99aa9cd345d538094094397eb1525c6bc2e61ee07e9f3596b46c2fb6b012e49f4be67f84383b0c10086e13b2821a6d7" },
                { "is", "593b17f1066b1e76cdb9178c67c3de06c0eddbdd2ad2e52f2e45a5e085533a2d1ca237548c9253a7a950a4508305a871a4f79501ef3cc53f55177aeb36148716" },
                { "it", "da6fe27b53eb251f2c5f884ce2b6d2ed7313df8e43303f365577af9793c9b3411160395e432f1dada0081bae302ef1927ca0cb7f308f28fa14a7d194b8843772" },
                { "ja", "3adcf8ed09fedbd8c8a2483c5fa884629272fb44e6654ef5b0d35ce47603b45255105372caea979657acc49655e6af25d056730de823c3ce46b73a9d51766173" },
                { "ka", "a84548daf1a137675e482cd37f0d795d045b04042567fc524748b9e10e299a2f126336c6e2396a78bd86a0bde92dee798266f2b7be166e2e13e1b50442d1f2c5" },
                { "kab", "9eebb2b1ea63b53c2de1d9d51081306fb9b5c06e8e551a554f320b7b0224e90d2daa7f6191511193deb7216632efc77c1d9854de8b3c1b7a213096d01df45776" },
                { "kk", "f59d1b0338527ab79322dcbab87d0a2c202d6abcd6b3a70855c4392fa7d5ca83c46a89f72a50f09374b2be7efd6885f2aac73c5e29f3552c64e4a334fb2d37a3" },
                { "km", "4abaff255ffc4b0133a214dccbaf287ee1b0f61872683532837b28cbd782496e88830551bdf9a12639cf3cd21ae50fb93fe45daea896faaa33dbc1a8b2a2fb6f" },
                { "kn", "ad841c80402b5b97c5359290e5e372dc6c215350a0d48c99d6a390e267c8617a932d20bb4cc6ac13c40926801168d54a84075e3e16789b7880416b31a28acaa1" },
                { "ko", "521a81aa6d96f411cca81b74a91d1b37c1a0192a71e7f000a7bd9cef6db84c234e77528f63261eacab1e8077be2df9889fd3972eec5973942d3d5f23bd717803" },
                { "lij", "879156eec12f367cda42d3115af4c5a7aa8ee16f44b3120959d38557bd9c7bb3b2c8ddbf332e172bfb01d74edfbcabe5b02ca4902b62408a3c549312a4bdcfd7" },
                { "lt", "b03b8baf81fd210fb26006bce0fb4c9573957d76b1e200455d89c9372c6f9ea2758edd9f506f3c6b7e5a278d7a10bf665786d54b65654637869de6522d38ae6d" },
                { "lv", "efc843b809fb3eb259cf0503986462f08c0352512849333da837f2f364ae474515752bac2dc3c16509c877a9b44bff3f81c5bc6465a4d71b3fafc941c7562bac" },
                { "mk", "599dca381de173eb12324fc6b891c34b1812b4923e3b44723e233612cad252183acc78756ef5e3652a61cc52014919bad069147491bcef179a20994aee47b2fe" },
                { "mr", "bcbb1eb2b48a91a3d9edb3279e131567fe84684bd2ab21cc34796a1eee0589d92fc1edc81a1ee2331d5ecf193b3f25d55216a0304671f7efc71da5edcd27648e" },
                { "ms", "76de1722f7abc16d024d987e62ae1cde5a9401141812e82ee58d2a6992241ecbde7c345382da7aec2248f015481a464920c288ea31b8e8181de2627b4fdee1f6" },
                { "my", "8aa8f5eb1f43bcea7c256017a146d4710b2095589bf30a150abb8284ae759fefe03f4f63ee2f84860605253946f25f2397a8a0e46d63cdefff20ec5ce3d705f2" },
                { "nb-NO", "7c9efdfdd0ec65fbd40dbd9f25f54a4993691246fdec355b6c9984a5acbe208aebf8d854ef6e0e572b1061123317ca0ef20515a3792f6fd78e3b145fe082e76f" },
                { "ne-NP", "c4a3bdaab229b140051620f92f074664abf97fe76a61788b0f40674a529bbda15de96fcf230d3957a51338e6de071c9b2af1b987bbadb03fac1fee63852a4767" },
                { "nl", "6dfd1faede1f108c6fe67de000fd797bf733cab81f37dffa1971161e39ce263f79e1dfe09d880b50eddc155be48f8697200c7429e3a236719e5c152036ef666e" },
                { "nn-NO", "f66dfd57cb0cebe4d373c7ca9297ab92647031dc1a74324184cd50a3c50a9639786f6ce533276f95c45495904317bb66c213e0277baedb0714630120683997f8" },
                { "oc", "a3021b47dbf943f5f448d73a357ded66d252d528550683904e2833e1b6e6653e4617dcc219ecd7185a096fc2df16835548088d9acf45852490487d31100031e0" },
                { "pa-IN", "93d31f63f18f0d31ccafe978251d9ae5645c3224746860249ff48a84411d1be2e089e04691e111eac51c986baaee9f60f6d1f8afb336e95c3abdd15476dbde16" },
                { "pl", "6b100a174fda3e5b94d1c91d48c2b8de51b21aa63394899a1944887151fb47eae0f2859cbeb3bc49116b94d03d4f2bbe7a94279706d1b028b8711f88ecb9527d" },
                { "pt-BR", "634e3b184d5bc46ecd252dcdd0a8d8b936b2d00718326733a8523b8a1738344c47d971c4d0ab812cb9736b9425b736828601b708ca23d639efd9006ffbde4cc1" },
                { "pt-PT", "16b3c374de272c19f5643f1e4a1e9c0f61db1aaaaf22c8470fe2636a5684b474152447bf401b427080e67307b3f1edbbd1b405858046abb0b68a237daeff898e" },
                { "rm", "bd27aa49aa2b31be51e91ff82fdb9cecbf37ff41896f71e802bc4b4f62454eb33861c709255f07f35352e1cd84d492a14e217a4d231e8e4141e63e2b08a372e0" },
                { "ro", "7a8f2516da3ce2f70ac8abd2d79a70ad566d63d83deacc1f64ecfef2396ab85eb7cb55f0380d8a1478c012cf39aa3b9f801ce81aac874a34e3b7d8f72844b4d3" },
                { "ru", "3d7d406b8905361aa10a7b498a3deffc9b2052f36b5d197673febcd8d6770166085649322d3def759a3961a7d7516f7ff4734f024769f7870a3a92759a8e4624" },
                { "sat", "916c4344cb87565fcf0a149420182174f6c8535a570658b96e42127a1e471358cc80376ad9961dafc4d67deef3275d44b670bee8eae11cf0d52f8c869fba3cae" },
                { "sc", "4c1fbcba2c42d91943b6aa25636a3b108201be205dae4d28eaf00ff62168f4c8655d70066c557e82e1fa61f653a061a8f9b178e47b0ccd6a09e3da38d25a4e6b" },
                { "sco", "4fbfc589bbef68034c4391c79ea826576957a8248162d5f79a1300f50dd7ab66330077cc049a210076d8d14c905cfc51a0fd6698f27c69adb7fefe3e6a18a8d1" },
                { "si", "97616043cc13dc890609d92c1e95b8675901a93d2beb5659d756c6a3d5955607d5bcd3998df0020d4e3183dfa2dde7e31045ad0c1d9a3a783d4b4b88210c720f" },
                { "sk", "ce233b35ca5662daf4b76485b5bd0ca2b6867f42ae9d8ad33c0b1d1f3ef6b56c025aadf1cfbf0ef879e568e0eb30a526d255756b4810e31cfcbe349fde112f6a" },
                { "sl", "c766673813863e5c397d61f400cd4dead3d2522385617ec283f954aa834dbf44d2baae656b8170a510faf2ea96926aa23edf6dfe5755fdb93f94e1fe1b434f97" },
                { "son", "f2a63623f019947416eda20dd0cea7b0301c0d67b413a6591842fcdcd84bcd53d12f25c889052321c0c43c44e64e828b0e85da6f57b86c54f33473cd55d93b9b" },
                { "sq", "f66d1ffc617775506c51fc9c9359206ea75f69a1f295bc63143fb92762a83d988e75f0f4a70f755d1214866c27308e708cd170c70ee20b697d0e5759ce5f0dd0" },
                { "sr", "7aa37bc8699c09e8462eed14fc00c8c3238b0ad4691d74949b65036cbe58e56f444e2d85f9f88a66be2d11dbe2bca7c3fc03eefdec753e29d54b824bc68e87b5" },
                { "sv-SE", "5cdaaebc793b59d105756485a3e514e14508dde57a17f5234ef3e33c4a4948dd2be9c13d62168edd052c536701a49d73ee8e270d4f58ff6fb22f960e8a6fa61c" },
                { "szl", "15aabecc5257daf4514f4df1ae5bbc259b27820677a3870994374175425747213de29d82457cf1af7ba0596be386d4b1dbbd5e8d1033888d6dc4399ab4d7c34d" },
                { "ta", "f61134320b1b480b14978536326a35e7f83bb41e5801cb0c2cc6e57c3df0d6599396473a2be01491b9e0bc6bb3874322d4645082e882b7be7e7c2fedc8f6aacc" },
                { "te", "b44693dab606e926aed1f0f20dd18ecfad5bc84a9220a8ebdd7a5f7c9e420942efe0ec6adb5c5fe9c8773f03ef9191fd0b94d1c48268f05504842acd603ea86b" },
                { "tg", "29fa9f0469ff5d88da3110d394b5b693be034be466ea02121d5a4863402591e9247636a76329c41081f409dd7baedf9a37a2aad6300eeebbab10a1aa1077de64" },
                { "th", "12a2f46764cbc14e3b28d691868dfe8d7a5ea1b632368afe545851ea15593dc6fb319630d31d872886810ca95576902caeefc9ef23368d541c8b05eb3d765010" },
                { "tl", "6a7ae6c2eeec183165b608f53a2d2f3d07e88ee45f407fe61d40ccc7e4b0e8f156afbd3afc9c7803357fe173720a882a2ca294a6b2609b8dc544046af3873be0" },
                { "tr", "8b2fc1273fddce0e5127b5242cae0cb0698f5f5e2d02fbd1343dfd78972c696e0697f55ca7fbf9dce3bd2b76b6e9b28871070861b0acd8bdfe8a7983fc173655" },
                { "trs", "f2294a0e50d6fbd60ecd20d99ad61861778f4374a5a17309bcb88784456db44638847b295c36336a361a7e00bf6ab0a6c1ee98b0275064320131d6fce330a521" },
                { "uk", "0715cbab2991666bcfd6a995bb609f4b851d4463fc44c82cf560540eb2050e2cdbd268e770273a0b338fef654600d9948e3fa03cd5dec7b71f3aece65a02529f" },
                { "ur", "4a10b247e5fea2220bd9cf2e6ce387f63557bf717fa78432442b2d7143d1e8bdaa7594a1e76c985b47563ab5647f2990d36a3f52454d247731cdd114388db41e" },
                { "uz", "3fd3c55a4b89da3747acd2bb417b986b34ca14328683069c934519d3620500c647aad8076a183f805327899d941b62bc7111e1c9e7777e4e5692f36e36ca568e" },
                { "vi", "c3267e905d36108cbaef8e9c908894a6aed3ee3de2f267f67c18fc27ac1eaf9623956eb7da178cfb628537f08681272c3c2d18936edb303b557531aee8fd2106" },
                { "xh", "7134091b7340c7e02cb56e62011a1ffa2ea497df92f36e3f7160ed27e32312fec7287f1f4192843e6fd405512407663f363179c3902056ec7a52e2d6526571e8" },
                { "zh-CN", "1ac679f78541f7c4b21f6dcffcdff6cd13b2a4245dd28be2ba4ff21374a76bec0fa5846cf9a295325f7ee13e0d8a8a53a3d52afd59741ce9201d46924e16cfc6" },
                { "zh-TW", "d0980c865d6614110ef6ed45b054c3879ba6e58dc51cae85676c6536406be20632bd677fe6a917d3020f322cdfa6a6e212d5331b274ef02bf795c365d0896d31" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "4a305c8a6fe9a25fb1f9c818f577cdf6dfbf775a58d729684f84ae11121a03c8c0b3d63428c0694149baae6d0cbd86bbb51e647ef0c69b4fa25e6135162b67a8" },
                { "af", "c95b5e20a285b2854754ef430817b7e3ea8777621cfa872830c3e63afd14e10a4f52b401b38bd36db37565c606dfd371c53584d7ed0cfab64aafa48b073d9a97" },
                { "an", "cb3ca7754927e61877c8142e0e89b52ca75d57b3ae2a80180de9ecccb5f7f63289f5b687debaa92c9f5fb3ccdbbb92aeff258411623c568c74bfb7b4210daedc" },
                { "ar", "58a1a89dd2e25dfee5205148f60d255d5aa9fbd13929ac4dfcb190a81fdeac1d0b92a66cf26db771c5c7e1cba1578aa15aab6ed3a3a138571fe3937e6e1d45a2" },
                { "ast", "6d0c327deb06d0165e22792483b335d1826d8601da6e28ce50c6515ab3ebaee1e43b50b8b677af253413b2570df3a61021f8d3c404a5d2d964679f983e526950" },
                { "az", "ffc0f57ce5af486ed0889c12b9c368326ac5d6be9880c1481a78650700efda81e7316ebe17dc6cd1eaf9dc920f08a6c0e60810c4438c24c10920e48859ba9176" },
                { "be", "06517ed5d2a24be4a1aa2c28fa2d6815bfe12b1d8a4358b9c9cbdb87a075bf13be71c822002bdd5df5b819aa99edd6b9d455ad6a02aa9e1e041bfef10df45725" },
                { "bg", "18501c9e63fdf0eb3c3f0fe70b14298e9069f414a47a176e746a8d248286c0a41f38492c3c7202eeef7f59ac43aa6178a7310de80274a622c30e0c39ae98d060" },
                { "bn", "0c6df7aef390c822f1ead3150d5ceed640e1d11f04f192096c8feb6d97bd89c52fb9d8b0f3f127270aa649804bfb4def3dd7930adee3f4d9c0139121860bb1e7" },
                { "br", "4b43fa907a1c71d650309e48d19a5049afbf942375421118db2ed211ad08dd48faa7a6c7f361536e50357c169f9dcc0f80b5f825727501a4f8765cfe8f8f5456" },
                { "bs", "0af8fa366825a3c177103f9bba74f07d040a93896d9f085f39ac933b8df55ff483dd16cb7677b241263d57fa6888f26dda837bffe8f1ee348fe1e8dae5683ae5" },
                { "ca", "f83c8471d70541903be60f6b967012e85821366669507dca39a4e51d1f557f9c627837eaa992f18ca110c588801657f9e64a00de9149ae9ebf1b6d53fc6e3317" },
                { "cak", "ca8279fabc422cf5fc1ebd40df802d8906649cdff4d063816b3698b9929130f51ed9b3a6b903f2b8bd7c5f6c10239a5709ceb371222d696a85341fd08b0b492c" },
                { "cs", "f07b8d9a8b4f3223a2361c640f7b7a88a8e7bd59a94c840c44bd176c5b929503579dfb23596b3416c7d0d69c84819ea8de0b3a600d890dcd9d7e21b0cc9c6541" },
                { "cy", "6eb946e84103a05f4899e128d4edf4750cb5d8f97620716daa48df6cfc6e3638b5cf0abc7194cfc01dcfa7953ccdac68c6fa45b1545e7fca19b46b759a0120e3" },
                { "da", "84ada9b7beec697c143f92be7289673907bd14970ed6994327ac0045692bad664b47c084df1ca1528eefb36432404e35ef313df459a218649ceaae10a08f5875" },
                { "de", "81d47851a7b2e93d8f71ea0eaa794da970f7f15b04cccc43f624bfa7e58d3443a3601c10467f2be98909ed0a59fe53acd4125374d48267ede3abf75cdcb03000" },
                { "dsb", "9cacf221ff65a66a5923e17acdb4596bd41346d0fb1d597491ec2b2810730998bfd21e7d51847eed764dee9b80b5f9c3ba4c0f625932cc2f6c59d0b426893d46" },
                { "el", "195dd894aa106eb064cba90e9131479489e8fdbf8dd764f54947221f922ef1d3804ca4b22edb331db6d3834185cc88d19eaed06486f0aefa15a578840790639d" },
                { "en-CA", "8eb558251d989674d1294853236689ce510c0b7e34e7d8e49b4fd8c2b68b7421cdbd26cdbb98620750bf68646f70c38d89061b6ce576ac205dd16eddd6593cad" },
                { "en-GB", "6f1dbdc7e5081be4bec4573683c75b5440759fe82ef87247f7b132feeacba8dcb95d69b0429a9a4702323729b3289ef7e346abbaf68e6b160f4fcdefeacbed86" },
                { "en-US", "fc0647e672f875a9174ccaa1ee51e8106ff8f71be806d4ea41905068c6cf6692793ef29c2237809825b6d3dd7041ea75e288246cd41d9a62ac76b1db15247539" },
                { "eo", "fb9c28e7811606ad9e46f9e187ea0ba9ccf25675d70ff9b535a5e1198b4fb3cda34c49b1fecccf2d40441660d98e20c04e26d2873acf007512dfc8ed034f64cd" },
                { "es-AR", "13133204e28f608962d8d1bc26cc2082bb61efacb77ed3149131cdcd611145c6fdf8592e05d7f261d2d30f516f88e4d2b72adacdebf66a546d43bae7002dda8b" },
                { "es-CL", "34c63da1ea9d09d4f15e04d3a854c206279a816bfe9132855d4d4336b583efea83eb80945a3f925a2d9cbed0452e59991ff21e9603c096620c28cbfb16d10d74" },
                { "es-ES", "1b4142cc02bfa9465c79f2d70d6244242c0c9b829bcfc414858ecdf35d2601d22d63fc0fb0479884fa0721bad3cf9d038c225f2de66043f3758c87aac5f037dc" },
                { "es-MX", "59ec1ef1adf6e77917749a4003ba74e762dc7806e879919e43416d2f3ce6c17fe1539670e08425c587bbcfde374961719b97362421e715d537d76d5bf5ed5f01" },
                { "et", "da2103069a937c8f0a7b4afd46c51152f32b5965e3a621f6e6a589a7a9a48b01fc01db8f13cace68c34b3202d67fa81696831fe679fb94f49ab56dc96be2c96b" },
                { "eu", "b2fff0c10c5d51fe84903dc5272dc1c9cac281477d0c829141df74ac4b48eb746d7f7b45bffd3211d7e1f31000cce8560e5b3ef3224ab0bb481bc38e9c5fdee4" },
                { "fa", "280c4901ffcc9e4e96c4e7dcbad9dcfd54ecfc7b1dd5ad2481f58c958c54afffed17d1e557cad1e03716212409fb51445e6f86426ddf830157443d13f26e89a1" },
                { "ff", "bef8d02ed1e2da4b843af4711d735fc3c84e6252dc7048a433960397459a699dcaba89bcf57736cc36da30abcb83d79860c5d3320324a9ec9ba264a83a1cb45d" },
                { "fi", "4269309ab6d5ec4e6b7e6998b67c172bf8d98fbb5ac1662bd147b41c32632c37a040188feebf8c8bc1b46e90c6f005cd092f50e740b8f611d4c046c65a5999af" },
                { "fr", "b1f86eabe67d9077aff1fcd1b83d557fbf022a9413dc0f819e4d575e141ea3839c4e3eea09ebd58c658590c80cfd4b2b5955d3057f2797ba891602a02d030ab1" },
                { "fur", "0678dae69b37ee42e7914b3970fd83e1d0c07fbc1ce1ca54d1cd42f60a129d08b561715f9464a4761a55cde9050dff944779301b1f25364811b6463baff55e38" },
                { "fy-NL", "01fbd96866ce124eff894c1bc13fddf7e0c148d0659a101e2825157f6a40c0653719b19dbcb6c8b1711fc3473bd56036c80284687979b18892c498577a49ac7d" },
                { "ga-IE", "e00e5979b6e7abd39afbaae65889d7ca6f4fc09b421dae6f9ed4d81ac739a5e0c965c90a203b8c0d7412b7518e15a2872e348d27297bbe0b338d45181cff038e" },
                { "gd", "c3235284b1940c9b136e3757e825caac6e4b491815f9469eb79e4665054b38cf8c299699b5d9497aab0cf8eb5aaa320b658dd82a15176c75d6398f35f98a39ef" },
                { "gl", "637442e9570c4c2be45e6f108b3f8ad5d9d8d70cbb0407b7f3aac1af1eddbb2047796943bf9407219f1a6e582d7db6a77785b8af16b09af8d5dcb0038972fc59" },
                { "gn", "146127059f7d8b4c5ef30c8a8a6daa946e487d9aa2c0078e26f91f598bc9242992795be1c53b2525f3316f92632d6c9fb511c54516949085afff189bc5f2da27" },
                { "gu-IN", "9d8270c06b268724b7674076041aa49cf93c0870501351015f4cc4a03c4a51a88d86523e0e7668a9fdb12affed1fda74acff2b17fc02235debfff5c75aecd1c7" },
                { "he", "f9679b98439681998e137173ef3c1bd7f3f1f13f1c3f87fccf40cd8efdfe93cf591cd0fe4b913fb926d583ed04d4474361ed02653c7a2c68c3c8863ab664834f" },
                { "hi-IN", "a7c7bb841b29e1d549b72ce9ee494ffedc3f758baaa011ee4876c8b348f6179f36f713ca9da55d8902d0efb8830abd2b49513f362b0072f94d32d29215f5a5b3" },
                { "hr", "8ed1c4d3d3aae8a23df345873dbf496bba0bc1a34fc71c231b6e821d46b3508dd7532a79c6f5aec6eae717abac8d7d74983e50decf4f29693b6f2fe929b00045" },
                { "hsb", "4f23e20e191bd2f6a6ba6ea0e5e7d6bd3e48d0488023d32fdc7c51d59332f2e40c144d077bffdbe653a26775ab77f80cc4dda285df985395a11cddbe48086dd8" },
                { "hu", "536da1081999707f3dde5244954f48706ed029b71f6299f6bc1f215c36823a55e3406cfcb0a4c6c312b0a425679229b800548da34d44a6ede8986395f80f4b42" },
                { "hy-AM", "37f6fd40812cbe1f366e125ef250bc548334142ee2aee52df7c81b88fbf55e22636a2f3e47d89cc63457c4c7b6096a1bc7eec85eb947535dd4bcfa08501bfa94" },
                { "ia", "08476e4a65188a3dffcf69493dd9bdb84f9cefa6e3d418d1547dab318e89354e68d28bdb5a5e267488ec4aae40c53789cfa4f07a35083b17c4f125e9b7fd3edf" },
                { "id", "e7334016bc604882754a7bd51d752fbf86ce21539cf4c32d7da7e10f5e2e1f8fb9afbe48b1d790d095335664624dd9b94566fde8bbe960c5e6ef7eff5f04cdb6" },
                { "is", "15f71f05c3612341509acbf374778d70d916241f58fc3f4d1399ad3834c3305b80ca5d167376158b027f7affeb7139d184ca50d13d9db65ce2c3a7a8971f1f37" },
                { "it", "95d7f9088644934de1841bf445c817e9cb97f30ba25987405667d6a6e90e144a40717ff6c80efe5e093250cb3e6c04b4be373f788fefe48a65e7a744c311e925" },
                { "ja", "ca9cc941f57f5dea06b283a5b48b755149bd1cfe3ba217a3af2b3d6ca7008bc315d7ee36783a3bb92e53bc4ff8fed1feb254a981fd69fb07e849e0b327640504" },
                { "ka", "e5d54fc1453a4d7a4f956958aec3c72356773a7bef1d15cb0e3d9cda97380dd6055b67529823b98f5c7e562362099c415e3800e9bdf9cc36cb0b440b83a71800" },
                { "kab", "5a01399a2a6366547c69f7b51a10d2e772b3805e278a3e0cd4229d2c1b410d1d0be04ec9b53947df65f0d3dc6a5bf2f41e6bca640ab0ac676349c83a2e6291fc" },
                { "kk", "5c3fea2a3d85f410f012fb3030b4350c9fd5176bc554dc675e94471370c7e2da9e4b279557a4d11c9ac590a179efda243e194d355ba6606363aa6b8d9fc5bb23" },
                { "km", "fbf9ecec887482e8684faa5b28ebd1468944379f64ddf170bc4b27fdc5b42150656076786cd67bf8fcd75d69b66de3d7c503a0902e4051313c6543a95496e6aa" },
                { "kn", "9e230174f0730c99a11a4b35a38a2743cca0ee0a2430cacaa8dca2b446a2e5110b82685211eaf97d0c3e61ed100babccc3d542692bb2dbee82cc7e9c3daa8327" },
                { "ko", "24bf763546e8398056ad80b05ebe096c04ae7943e2ba35aceefa3f599aa1cb0b739c7a3c8a9e8022620a609bc26da675ca1b53b2f0b69a2fd9e601844a55a40e" },
                { "lij", "9af1de2663f0dad8e05d30dfa909bf530b8cb12aa33abc4ebc06096cfc5a890ce31cc6ce6da6a2f8b40f3e65a61f11d6e4cb280258c5d2447e56734a003fa2a9" },
                { "lt", "b374b432231665da568af17de2fc3d97883aa2fcd44547a188089bfee369b38b6daab28b11d82583b3c9f8b9ab983ebe1ca93d471e5a58060bf349b936cc2fa3" },
                { "lv", "75c4a8415cc858d1e3be2d2d8836e92a4fc9b8c248e5b51fdbe3c1eb9d66fcbb4c7c6a6f8b92cf3db4feca0100649a558cea9f26d38839a2e99d625cf3cc7724" },
                { "mk", "79d5251ba77c470a706d91668972ab63c770d168b3f0d1f5a018b1f3d68e28e1a609d614e1e4b67564188d17724d6b5334689f208e26e528c3e74a65da77ab76" },
                { "mr", "a33c715208e50138dc923d370098db0ff68946c38cd16ba0971ac00a1e901c050e53ba4f415bdfd7fd942fa150835ad7f2bf669b664576fc0a106549b14b8e69" },
                { "ms", "a148ebc08544a1accbb173838f7d953a10f33eb7f9d3ceda595b451f3fdf56765116657aa4c9cabed68cd8ff9c8798045b047bdf8c256d15a1e80dadcbdcdc09" },
                { "my", "e9f1f6ec309da61162c9097568e672d38b222c0c3c8f817dc654deabea259795b8f948b0fc7c1b6efeeaf8bf13adf5d373609be6a1cb30a319c97817d53738c8" },
                { "nb-NO", "54d65e7adfa1ada293e47aadb312990032b3684f2a35c8b16fba0c543b762a6d1c86cfaf2bd3c4c2d27cf224e3c5714a2cca8e7a9f30b88904af9d55f9438792" },
                { "ne-NP", "aaeb0e364fc088cfd8a2190ea483ed83d94659db480f935f5071df663fd0cc9c3cd8c10439a03d504308ba38810a3358494f6fc902f539880500a39cd6f99d04" },
                { "nl", "f284ce3a68efdd33bf9b9bac36e907588ad21404dc4afd2988d079cb2f27d0c3dd0f5ac9392a62a940e7951db9060d08008f584965da5d6f09e7155739f1b113" },
                { "nn-NO", "58c4fc516acf51084268d972c9ac8c8d10b3dd9ffd95e35318e13faa051f813c8ace74ed02307bcbdf321df4186cd237d62063d447cc5e3ae24e289d5824ad69" },
                { "oc", "91b51e1410fa2769a7b9c8e6919f726b352d70bc8d96b87f79856b4703c3e09574ce0554350bfcfa979c300b0c5ccd82edeb9a5b99cf9461fccdd8b3ac37448f" },
                { "pa-IN", "ac7aef57a377d0dbb399242fce6b239a4010d1553da46216e9e30b66be36d938835f653ba244eb1da55f9494c497e4a0bf379bd986eb3a8c3e6a0e06d0596a03" },
                { "pl", "578ff36f321c2969943cd81fc93421f5178434e471bc7bf8cb2bdc6f0c65ac38e2e415fcad88392e58c344e54dcb40264ad3279d0ed2bb015f680a63683e3fd6" },
                { "pt-BR", "59f25eb570ba4afe09cdd7481f1fbc6049c736b949f1fdfaffd26a90789e490ad29f92e929653a8f0cd69f2b85412c7427ccdacd9d887e7823b530479729b72e" },
                { "pt-PT", "26a7372eb052c5ffba74cf365b96ed0afb00f46ab4dcb9fff47ba5252143dde4d9772e10fc1aac22dde49e5c616d1451c0d88c1d577f10ad5478d04cb289f39a" },
                { "rm", "faee8f95909bfa6629806202a07e86891119ad7a9039c4853e26fc48aab8eb1dff7e1318c0b03878d1d5601bc1c9a24382e5124cdb172ed20ebeaedea7bfcf50" },
                { "ro", "9816b07d8667a18f9fe19afb21ec1b541fb89eaf6482cc9a66839d223d366fd6cf726bcf7bde8a0b927ae255537ddbdd97aaa332194184cf9a15cc0d1ae742aa" },
                { "ru", "5b217d74b3009231fd9bc540d81a186cc7f48b04a2ae3bda9a4ff3a34952a91c7d984401d9ea8ba3d7bf92e8ccfe585bd94e24b73e3a57e71787c54306669714" },
                { "sat", "070f7a7b97845d6521d20833d30b8ad57ce1247e0dc6af5ae76047d95cd83472da2bff82d1f598cd599929b09a586f383c9d138cd33eed8c3477d209b080a35d" },
                { "sc", "0a2a80495cef9aba58e8b0a5d0325755546c9824c38d15a707b386daa480361c6aeef01efc3b6f47c60948551791deb2c88881fed1398a451a9e405ce512f7a2" },
                { "sco", "c54ba734a3268e340688f77303de22130cbda6ff5a640bba783160e610b07905555520070e0c83f432bd109c3769d218c3f744057d9361f399a1e5cda85bda15" },
                { "si", "b76a9d3ab1ed7fa79ace60d1b2ed0534f082df06a3f79be6f333dae2dc10a33c170568afb1362af2559689b47d69dc00a964396f90da8adec04151ac79e520aa" },
                { "sk", "ca48c2c5fdff3d15be6ca711c99995ca44bdafe64db0e82413ca3d14955193377cf0ac4b43427b75713ae7de3e5e5df6f9613b01237cf5bdd8a73b9decabd8a6" },
                { "sl", "93ed5491ff6507d9390abcb7c67bfdef50e264a52b3a590f461177d034c06dd32d29ca95809d599d11b25697f90d5be65426347295e041bc0f18f698c76d23e1" },
                { "son", "748323282e399ee66937196df3af2843cddca0b70ee8e31cfb0fbe748d7d9f2385d965dc7f0c4a73cade0b2caf3c3d29cdf87bb31d6ac1efb512d1166198d0f4" },
                { "sq", "d8f0101cfa878cae88f726626ab5715d83afd36dd057c3f2ba21ebf1cd6b0c6d678cbe8c8a0de9d1c9066658357c74e1906836e37ba2bfe36785a6a9ada715b0" },
                { "sr", "4a4b5a9aa0258833e934d3c29477acff0f7cd6506a189c65ba9a91472cc32249ff470bf66fec53c798a2b826e8de413633858742da57baf493ffdb03ad8568e8" },
                { "sv-SE", "bfbd117c25c4c09659c26bfdb07a785b3bef09cdfed1436f9de51e3d302860833fe8ff13adcbe4524c60ffb744214da50f0276c909b5d0818096c03493d30fc4" },
                { "szl", "debf31c3b48d89547183af4e4da1f4d825b92c44144590db504d76b0cbd9d2e4801cae1f85f66bf68b80caf58f2cc4d309a479af273e5a6cf67ba9857b4db3f9" },
                { "ta", "cdf431b03223c98f34dcfe8868c84285606bef776d9e43865137cb79f431dae9390bea00db016dd91e7e6594601215460c5b4b5bc4b00417107abef77bc3e204" },
                { "te", "d84ef7a64016a8a2d6485ce8b5eb4a8ef5f2cac82bb274e56f57b7e31f60672c99a171a471e190739e07503e4f04d5167fa3c07fe9b44de9b4a2355c8938f746" },
                { "tg", "0a0fac728318d302112a6d1ac477daea694258712e99da127b8a3798628183173edbbeee04c8b6eb8fce2ff6caa9a7b83e23cf0a33ebccdcfbd531cb4dd542b8" },
                { "th", "cd3e26f995edf9fd57c4549b56b01c4515a6ce5752df34a85bbede4966b8f4c82305007f7e35486c20ec488d089826842ca3328ba589d678cfb69761b9d0fe21" },
                { "tl", "e2314da09a12a96a0aa0f8adc3522c4517e64d9e4c17ae5f8b3e580e2d71c9957c068ba74d4e6c1316cd80adf8b1cb08bc4bbfa73c3188bebea2b4cbde4176ea" },
                { "tr", "bb8eed91485d9db0a6df4ab70d8eda081e481834f2bbe4beb2e7f310be267b6851ac4c96a36f638716c79f5a882f29d6bd1cb94fc0ef6b7f4ab38a39930b3302" },
                { "trs", "891494bb08b93f059716051a853c47238a0ba5bc0f856b02d003db8a9159b0c1e807e15239a817f109ace95334b5352b52800192ab04cef47bfd505ae4c52d9c" },
                { "uk", "a67c21ff8f6820e052a231530faed29d218ae8546a8c7fd2491a9f4823fdab8ac9cc8dd7e7131e9ea75c4c1e4e4e48b0dcb0872b004b0a03306461cbcf4bea70" },
                { "ur", "dd7398e596e562de169b7d07fd873f970c9ecc4b14d4cf73a6e9192636cadb21bc987b26b869cbfc067058f5bd54b6b5eaa51f3f31cf15653fed4563c0d2c6bb" },
                { "uz", "72826fbfb43da688c7e15facc3268140875adff3c3d6656051ddbd4bda45bc921e600a6e4e45c1cd04877e5c21b6d4bcb5491de8e6e76943528ad58e5cd164e7" },
                { "vi", "4192859756be24ba314139c582e98d5ae9d0d5e33da78785287ceeefe4ca74f972ad10bdf4cebd5060acb5a7a19dda214b154f277dd1071811c0398d0cf85d2e" },
                { "xh", "e479f06411cb898130861aa1e1fe0add426d788ee112c3f97cd67cc3e0d2936f5cf636b4e713cd2eaccba4fe4a5d1b50be06112c1203682379cddbd8b5e6cdb4" },
                { "zh-CN", "67cbbbd2441c62beae08d7ae6cd00749bfa8e61904be390b3f7d7cdad12430155898c488640996986f6447e43d5ef9800cebed42253f737e2ccafa22146b211b" },
                { "zh-TW", "1e30d63737c1458e987e7d4579cc67ee08519852c18e692144e90e625827dc8e4909c3f2492428be39090d1e88dde2553a8758cfc35ce2938df5acdffb33d68a" }
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
