﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/137.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "6c98772a48358bcf55b14c898ca1024cf6de68de754c0e6738e79157140e96da436973fc671046e54e881f78eb6af88d8d7dfbd1fc26ebc55d994c62362ab9b2" },
                { "af", "2731a427025dd6a2ab03dca4f687d73cbc498db9ca8ac5adb40be00fc51baa9b2dc2c72d1ef8c7c0c3fe7b7457d6d5eec71083ec640d10da18097394b94fc382" },
                { "an", "69da4491b1cc4e06bc0f0bf1fd49a93ad4c430e4456378c3025fa592aca78a39a8c8050a7a7e8c75199c70d3eaa29953f72f28e1b5d793c376fd41d2d7b803f7" },
                { "ar", "52127895a52813c1544ef2a20bcb2c7d445d4efaf6659e47a77b7531543a8cd506c4043b3948797a5c4b11157c79599492d34e8f4794796e01dc6c5b021b03d1" },
                { "ast", "c55e6c196610dac832491f324d95b0702d496901f66d0f1aa856cec9e3389657eeb64c89ca9492353e54ecf1017e3b3453508b45af8c72199426fe5e51a0d55b" },
                { "az", "7d7384fd29e93d1ff90e44d05efa51bdcbf131b6741e0ecf5a70740a34d6a9191e4a42d1cd043b69aa30140a0becf31dbc9ed20c5aa9c6a09926cc352d12f8e1" },
                { "be", "025e577c0d950672703b19419b1ce4b04e42c798f7039ddb3db3bb88b20e8eab73e3f9b41d5cb7d8e26e0956ab64ad9a4292757c03ce294304c17c5929758df7" },
                { "bg", "1315ce64b254d784ff512788a6654b7fcd5e817fe78340e35958f314bd0dfe95a70e1d3db0d08f486118acd6df0523ddce2cd71c736b427968d093fd02fd3f06" },
                { "bn", "2873100dc3360b0ec6af29bbcc3d6c0c6ff9bf89d9918fcd6fc71d2209149045baaa0795c1081af76997087675754d62dc45accba9c9935c263882d6c328644f" },
                { "br", "22b529af4b4db419cd756d9e644b448b1848bf46ce58ae943e27766c167b0a5d61f812656ec0de59fb5cba487ea58eac84c4fbdfedcb05e380c98b6501404993" },
                { "bs", "708497a3c43f70608e9bc4bde373ac999871a6e255d227164a5fa0dab13d6e3b36bd71e810c84effbd4a7ee30cac9f37b354c3748c289e7a89f036b64290b0a8" },
                { "ca", "a2bc86fd25173f36612c50308ce54c4aa2dad177360ca7e9f79c86282ed938682a7eeb2a558000ae9fde85d72156fb27d43364411a164ee615bfab1bd3d991d0" },
                { "cak", "870a06c7375a6b243ff1f47c1b2a18513aa4158466162aa9c30b5017b33dd471910caa55250ca50811e85a2d8e8083ab43ac0fc0f007d37b7aab16ceeba4ce87" },
                { "cs", "314c203cc5e91a4d7702e49bdadbde029d2565b40d1661b815d1fd3eab0165296528c3a009c2e809d548c7809698c4fe2c142181ea28156f15493906e90e78ca" },
                { "cy", "6dd1c6491b89ce014b821b76defce504d4d0cbc11e61cfe7accca748289558be6c54ecfa047408641b1ec97250802de92d1b445b590282b238212a4bfa2b5173" },
                { "da", "5a3b645399568d3c11473370d1ded1a3a726c99c1ae4b676a286add112810881cb090ac3d30fa8a2ff41dc565922faf6dd51468dd9ab6fe821de49fce10f034a" },
                { "de", "d56e7c3b8fe83832d626078c92ad43d57c9ce97185487d3190c9189da681556535db179d1b8c64ee06e52b0f4aa5fe01fa1a3953c2518055a7cec130020c0b73" },
                { "dsb", "1449a0bc3927b25da035d3a65847e980ee7c7fc9c4ea151a27f69f8a6b9a285e966ec59cf721b6523c7f0b472141bd9b1d446e607d33f30d6f8aaa3376828146" },
                { "el", "73120ebf5e4ba53b84747c72236185a2f9eb449b888550c620c880ae71f8dc2ca460a6bf353b1fa75930387c297cb44619825d87ec2bdb74ce3cc840a6297495" },
                { "en-CA", "9f69d339ad794739055e489db1cbfcdc7593af51cf666a72cd07352c07daf7ab6bff57f5ce3aee191915472fcea93e1ff1e3e84eb5b25e12118ab2203c7c2f72" },
                { "en-GB", "eef7ca6c5e97ba58d54e40e9c755200f8b9d8c588bf2edffa1b453db4d6ab311a6354a8b4e86d9430203671d0b50613dc9553038f6ccf03b4e8ecd1678c5558a" },
                { "en-US", "70f87fbddcdc25c8bb6aebd3634ce97761a85c85952b4e1d74efed937730840c208c3ab1b3d1665dcd0b59d3abbbbf8c7ee2a9568296ddfab245095e6ed31000" },
                { "eo", "fc1a14de44895ba9424c98088d292ffe843ad68527e20698a6c6e228e28903f71adf3f8eaf5ebaf3d718c09ec4711eb560951d4cc46f16d1455da573e72da1e1" },
                { "es-AR", "4cfba987e7b25b0bcd5a5288b452d8fc7fa5b6b7d15e178e486d32a024f70eeeb9f91a619bf1b89cc6cb3e7d6173d1c73d68dc146818905e340a1120d2d8a3a6" },
                { "es-CL", "477949afea829a4b03da3fe51b66f49bdb0e8d9d28458b2e20985d28fbaab214e5efa9c0faa30c3c949e55616c05deb1da318857c25b0389194fca6a63e1fbfa" },
                { "es-ES", "8a4b7b9a7ca80e12e2e3abf2b13646dd32086fb38cef269acf03cccd15beaf89ba1526750619149c324331d1cd87af198925051e7da93b351fade700c47bb2cc" },
                { "es-MX", "a9ce42281c318b583e7123ddf8f84faa045bf83c941e3ca3c119febccb9864b247b018f7f5d629b3bcd478f47f4efbb9f7a0f6868e839671736ae848971bb85c" },
                { "et", "89f14bfe73705fb5378712be68dcc0f76f9ead5201cd4dcfed5927a2ad8a3a751261841f8fcd5aa2ca5b60f59b301223a7df51eaf4f5dabe9b5d0bea503c06e7" },
                { "eu", "f232cc18d219f0d2606faee5d4ec014bde2332db6253ee1d6207959bd6247b3a963136f71473bf93a5fd3e69c7201303a8085d4df91002daf095d9176e186c8f" },
                { "fa", "078088849c20e69ca3f7c344b8462d45660e3a769d60578d4f6b878d10b84b569a8b8611d4672d13b7659146132bf3b6f69248f2afab0c72b87737032d3dad46" },
                { "ff", "77aa3513300cfd921ca8bc550aea873fd69c6c0965f3de3be155475c7773336d8fe7b31f01f1da6399bdadcb1a38575c5779880869de7fc07d0f0ea42b465434" },
                { "fi", "f770f3db0bcb313a4b5708cc6efa9bd69e1c44d8b2942217181535653852f9e78a1e04aac330d74471c20c84b7b6da0d1688bde8863e3251f86b788693c873fc" },
                { "fr", "21b75e1164b217648961dbb3d5bd0942447f23bd52e989020ff6ece017348aa2449076c57e9ac18a4f7738bc972dd721bf54bcbe459ed637071ee22065563e19" },
                { "fur", "3dc588be4195e53d5350517edd7747737818dabc332b6c9f8b4aca56f6642043c1c11e1736b89140fba329c4cb938d53268ed7dcab1679b701c9f973c937afe7" },
                { "fy-NL", "5670afedce5e103e84717f5de41ed6445daad0471f0e1bd6ed221653b4c3d5ae9c55f5799eca38a9d8b4dfdfb533b6a6fa5816711f42a4c2cae3b9b4ebdee4e0" },
                { "ga-IE", "7da11925915ee7350bc5e1d753ab9a56f35066324fb8c34ba9bf6a6387eebcd92d4c9b74490b78e00d6fe0de08189abd64489a0d908df724a7454f1982cf5453" },
                { "gd", "3e066e984b14d1be58621140fec9d57a3f618c326d61da67161b371e3c74c00985762ae72e643ba60c0765eb324df3afedf802a5e3d13cdd05b1f70cf7ab27a1" },
                { "gl", "9e26bb3ade7839409f93b2b09afc5c2486da1e7d07cee6151a8416e00f328b4e7e3e89d5a112f636c94c6f1556ab05526ca3b2412fed76f90613377a0de9e92a" },
                { "gn", "37c50cfcd028f432c646966ba3b251470a3a1d3d03dd04bb9cdd136ad9aecafdc6715044259d7c6f7b0fd4156d5d692edcdbd63500ea599dadb5c922bdcb2889" },
                { "gu-IN", "730d5df06a95274364b5c729d141aab802a40ab332c541ec8b79ba4e0e5bf933170b991403505286a737c522ac69076a5fe2cb32afb0f3e624aac194eb7abaee" },
                { "he", "14fd92607b197d6157ead8b102aaf5d117aae3a8cbc4db51cc0b2a9676c6f7c55b896b2684d7e17324cfd928c89cadc8b66d3d6fdfef60a7d6bdb52c2e464c56" },
                { "hi-IN", "fe400c311fad6951a68369f00be16bdbe1963e23edcca576171c9ea6c3f9bdd7c7ab8f318cecd4e1a01f29fa692dbf8535af9b7f78e24308808b420bb83acdbc" },
                { "hr", "483410c2e9c0cd94f132bc925630ecff4a8bd7db35e29c98777c24ca603ff09bf16ce76566c83177a32358797570177cdf7a510a9164c7678c28e9b67fa590f2" },
                { "hsb", "cb0059dcf9107551f0617d7bc8110dadec20429d76b7c2513dfe102701a951363f687f1bf1813b9919aa5ca85eb3ac5c2927a8793bc1022b210deb2bf6fcbb78" },
                { "hu", "4e73384897e62f55e6244ab450a45e6cc10af4b32c10818d9389b78c7805aa48beaa171ed4dad1454fac7cff21efebedf78750b5a15c4c08a258be0ca77c6bb3" },
                { "hy-AM", "fe7bb9cc0c648795127591e86f65d10693d35793c30a85a5dadfe9a59735e83aacabd6a83a9a7a3c8a14791a2cfd6fc244d3f9c02015b57e70b986794c5cf0cc" },
                { "ia", "f8dbbb43b4b484be01277b71ea38d7ea55d73be6bd33aa312bae3d3de49f88ebbe9b794049d427d3b35148c67fc82c305313153edeab1f22f1c7a046081cf6cb" },
                { "id", "8b40f118a6c9a0c35c9c11f2af5d993617c0e76dfbafd83fbdf4510c0aec2ce70f794b8ca7b3cf240dcd6b4c52f79fa79c02e1a843bd179fc2aea4b89ae08c89" },
                { "is", "46dd7add1c5e0896b7a85ed58ecf25e9dd3b86a6757c54394ea231ceef19ddc156ff7300daf0727c563f6fa5703535e3fbc1818bf599c3d7a4b5d0fa808c4da9" },
                { "it", "978ed89aeb69b864891325a13ad523e2dcacb170cd9489b4cba77caf05ccb7dce938d9e7d308695fb9e6f0988b4e46fdb8ed3f016ea83701db7db41e13b13bdb" },
                { "ja", "466d8834470ce53c411550e36550b1afd0b65e5d93318d0ac371995391f570fe79cd48611a4b4bed8e6f176ed44844e03e3d9adb078d7eb1f4911e8deb7a6cc7" },
                { "ka", "2873e1eb6e9e75498ae0c525141faa4e03dce2691b9652c5371d747466150cb4400fe49df16aebb95eb49e7c881e1530542df0f9ba468053967d4d864c381947" },
                { "kab", "5acd668f6920b97cfc8ade3acd301eb8c9be5f8c173f14196c1f3218d1a720b928292af0799001e3aa7c517502c4a7b345ad160a68efdaf593ec3f37f4ddaa64" },
                { "kk", "7052ee0943ac218a5ab3f8b6a30703c45d5d8ff7ba838536bafc260541d6bcc2ac18da2937e92591511ab697874848e93bd80ea254b5d84cbed95a29bb545dc4" },
                { "km", "ff0252927e8a76777861254f8f17e1188adf9b48e0bf319770aab1e0fab9d859957671519609544cd0e0e33f85390262213213ed8e40463d3c6869b065875f28" },
                { "kn", "0d37e797d804b34a3b9a5494b460a00c112d0dabdd5fd17fe5d84280295d41043a5cd152d8cb7444c1069c96670122ae4cc08340f815f391282f31770b62c8f6" },
                { "ko", "c16aca7745c007b1250727e0dc07430098932498f9bbe667e712a7209a400535c2c88bb4ed36b00faf7ee85017452720cfccf0eb4958c7e9c7544e40728d3da4" },
                { "lij", "cac6fa26498e0196080e303503b1ca29ddcd354d60b6324624955da36356c48b4ba5302e14df8849c03d580f05295d39dd282e48b66c7e76beb5766de4cdedc8" },
                { "lt", "4dfa3b3095b81b313f28d0cb3e87c4db7b1c92c40b618155e8577779f65b795dcd450405ff8f3c462521b32cf25386d04ef066cf54a585e2657aa4133b3cad90" },
                { "lv", "e87d4b3c331f850ced058f6b1fa2aa30d63d6a679e3fca542e422d35ae53ff6c652928c98c56c6ac6023bd26626491fb749a8b9ad310930ecdd9cfef775e4ea2" },
                { "mk", "34d6bd61677b2f21de66e53efc235d954e2c77c3b587feaa14a6c3114e87fda1e5f318bdd38d2a3c6cf5353baa1fb147408279c9e2954474f6e7d1aa81888d2d" },
                { "mr", "3c9e63bbad47fe5681e87cfaebb98e840ac8c560d76b53eb5faf9330e3ab50342a562d48a52cd6180aa74507bb5b122f9dcba709f763a4c52bffba2758879540" },
                { "ms", "f1e801eb08996f85ec0a45be8f9a134810ab9b1d42f51c951ac683967269c393908a964aed57ad06018977dae7f563749270a3ab107d0b802dc944f28139127a" },
                { "my", "0384a19fe037cbde8da7ee1747349bcc98f051fb11374e24a4c5ad63d153cb4b9be4b663c9183ff5791d3713c8f3b0abc1ede39a4810412125b97e0711124a20" },
                { "nb-NO", "90ab1c203f9284064b5d164b4497067fb1d522a5e0b2f9ade0966a27b8b11ff7259e9aa58294eefc2046a0c22708aa12cccfdd6ed8743896f52a2f95204199e9" },
                { "ne-NP", "d4fdf3de9827b6df7c55609f9e0cc540318ba42acba5f72f703bf6948be98b04e92b51859828dc82c8e02a21b2d7511e7d584d7da0e4a1be883f21474892476a" },
                { "nl", "3ea07976933c47fa94f7b4e6d60917dc05423497c0a8aa5585d985bbd0a7f05c215d3a8357c4723c3cc9be9d7e2a6d757e23e322ad9078d88c6480c2db557d55" },
                { "nn-NO", "c8ea47ef1fde9cf2e8c43b1a1e036de61971007bf0c9ce4ccf3103b86054581939aae1e46f595cd02772bc7cad35defc5f9039f6b3c35a52e14a07d0eb9588c3" },
                { "oc", "2ccb8cc8c8b8918f6f70b6779ebc670f5e1c14fb3976d1ddff81b4c295c046de9a917f2b4e933ec2b7a0c80248e159906ffbe46d4e17ba71aa64a60bddbdd0d0" },
                { "pa-IN", "19581674dcae329ceae88a9b0c5292975395cf38ee5dd912b6f34baec761ceb4be4568676123a3c502f7709f37adb91d446c2e0f4f601dac67d5e21a879bd333" },
                { "pl", "34bcb630506573b81dcdf48bff500e4c5990ab10aa9eb01491f169797e2fef997e27f50a20429224cd04b7147aeded121ebb4c228b9d40be3697c09a1837e8a9" },
                { "pt-BR", "1efe23fd6db942470d0581b9f803a00a11c835e0d636d6c142d74e241ae2400351767d564b54ba1d49f469256051d63a0af999a3116747003850707d466dd4cc" },
                { "pt-PT", "343c5910d99afc226001c4f733513c4bbaaf85ff62e88d9e3b02e8f80100dcfe784382eb6d0830d994da0e102824693472e02668164c312b7ce10da772e5be49" },
                { "rm", "c6138a6ad6c3481d508c2f8fe6a06c27d982d5eb1d28b73a9875cf13df2f104ecce42f6fb7640b4cc989b5236b3b7cfec6dce8704e251f39aba8ffab948e0f53" },
                { "ro", "e4dab5ae5c2faa194be48e12063e6b7fece8ceb2ec57a2e7353bc674dfd595b7e97b3e37935c736b966f7a0ab41a14bb659c1d6142a03c847a541c377d6219f6" },
                { "ru", "b41122f4639d0ae9b120271ef2f2a7ffe9b6c1b11d68e869d03850deacebf1c53869b0acda34df6091bd6c1b7fc04d7ec5729f399807e84e506761c0c23573e7" },
                { "sat", "e5a2f2edffa3bf314e5bad9d29a9b3a7a8eea211ec5ae7616b2fea906a9a60abe10cef9e499d350a13f26b5f8415242adeb46b0a2973ae5e7200aaeb883ab5d6" },
                { "sc", "c50dcde3b909dfb2fa48a1bfce38b53de72a37936937b5bb6a57ced6cbf146c85406f7a60ce97257d8722977e5ca1560021467ec56d7d06cc8c810ef3ce8e2c6" },
                { "sco", "2ffb8fe03a88d24831c5beda70f8bf47e180abab6b11401505ab04aa3cd69592c0a6609096bcef4cc1de453d51abad0d9527bbe88d6d78146ff25e5c1dcf7b94" },
                { "si", "6ccae191ec20a0037eef1569635e076a89587b0f9ee29c049bee5a016faa40da9a41aea5a1352276347a6aabd38a11ab1a291c06f6dd32f5ef72495fb524be70" },
                { "sk", "4a5ee0167c5451a37939fbcd995fc91594037f5294f69f3100e5fb2976fdfa2a85bb496b9d213e9bbaefa2ee3f4255b71dd35746c25b4c0727b93adb7e06e12d" },
                { "skr", "df29c64a3b18b357ee339f82210f5c0f3791b7592f6af9e7229c0e165e4629b53a47712ad7b5101a4de3eb7386242ba94faac22c948d5238e6398b3d38bb1d19" },
                { "sl", "97fcf524c0d960ec4209218af59d1a7c90b4b726e5fe2cbc7626e98a7b9d0eae56fb12d1db635f4740cf8ac870eea69edbbcb30512afb7e8a96de08fcb4d8a93" },
                { "son", "54600c6f435c207726c02422ae81e157178f39af1058a10d90dc7c3d67e59ca0a99b7a14e5ab6a7c1a5cda5cbcc11bfe87c57448ab8ac2ffdc6158fed441e0d7" },
                { "sq", "486c776a8d2ff2f33d1b5b8ba1b005aa9dfdbf723427a6a8978072446bd2a3e039dd742c349025630dd51d27761dc59d561d42520c124c16f5267350216d91ec" },
                { "sr", "bb0308243fe72c29214a822df3ce48945100b0be43082272311f79caac73ca065b4cd895d81d32dc7eefc8e7bdc1c2d39e1ac6a4f91c13bac9c73a3f6ea8e93a" },
                { "sv-SE", "5587c42143c2f529e7f990bd50f35ce52337cae9dc76e2c50db4e66c78202d394b5a8a4af6704175ff7f64b104dd37ce11a2f8225798f5f7e06a57ad8c161e3c" },
                { "szl", "07d5badc377312e24c6a2db0fbeb74b098e6e460374cbcbd0b9da8a263ebc113c79cc975fdf86de200e9e85e4e21897dc29132af4cfb2fc200ce5773532cac1a" },
                { "ta", "7de3288feb0de532856e4d898f4b471bd3fbde9784f436ee7328ed19cf7e99b173c67c426cebcb57da4741a0815b5bd43bd9a1b9ad45bf7db7fe41c216746765" },
                { "te", "9dc486fdb77fb6a12e635181743e93939908cc8fc63af5644744c4196766d11fbf956a588dd4e21e9e9f4b213c848a45ae01f5b2026f462c0636a56a0629af2a" },
                { "tg", "ffbb55b629726bb17e7afd7bcfce88f03139f016379427695933c48ce86b29651f5d57b96671d857ed94735b3e181a966431dc8be6e2ec5eb84277ea219927ff" },
                { "th", "26a88da6d3d450037a16cfaa6c883df5cacc7909f690e33564ede704075cb75990aab5f7a4f276441f6e8f1058521b1ebdb9b734856f005ea7e32cdb4872d2c6" },
                { "tl", "29854d06b0e6f4704bc7869c86d980b26b8b991e1ae4933cafcff0da1d1c4d454ae12f59a7204f95425a4f38e917b8d7e076d2407ed6824d4fb914932174dbbf" },
                { "tr", "db9a7803be488d8fadb9feb561f4a4ac23356ad7bec346bd05f1a04ec29c84ab1cf18a94eac4e747f9e57f3ea7559909dbc6a52b2cb91c082122010ea86fc1a6" },
                { "trs", "79705cbaacabee793f1cfb7ec63eb80d55ea35a1899c1c62172175aa6df83d612e9bd6c2b62fccd38d399746b4151bdb1158dedbc2d3341c1c1bd950c06bd07e" },
                { "uk", "a16a5701d7f34609fc65a08ff32376f3cdd9930acf17cd12a6fefd6c2a9327639c7a60a0adbb4129350d574b5d446313a32cbf77a765b9747eab0df5d777fefa" },
                { "ur", "f4439e57aabdf156c94f006a28eb8c299513a709ca5c216057eaa1a607026e352b71278aeb35a75d06412730066f15350a9940f2b2871037ed5f21ca7ec78d79" },
                { "uz", "059392bc96e3d516dae1678a79e701977e0ac44d405069f6c7a6bd38b22567e22efe8162c8840640b2cf0b778102bbfa7a909aea24d573ae2540627226d6da00" },
                { "vi", "c620ebc01a44d0b55798b1d51fb2eed0ad4e9a94bcddee019ff85867ebd40f162f2a052f59a9941e5259dde43037f7e01e96e1201419809d3cdd4132a9d571b6" },
                { "xh", "398656640466ca32eaca2f7118b9d1a468c7710fe0a65dd14c1fa15fd24bfb0211bffefee39598478a74fb64e1372376295fb1f32cb321f12bc73349a2b8bc82" },
                { "zh-CN", "71104c27bf8d6f88d905efd91a7f64178187feb76496b6a8765fc0ed10b7a28d7bdf353527a8210471f8f82ccc29338b3d27f2904db8760b3a1e8b1a8a21d8a0" },
                { "zh-TW", "bbafbdfe1203fd8790d389ba01a4e0b53e5dc0433483e1e5e6b0c1ab64f891f0a5234bc542c3f63e96158f4cb10e7f1cf5583795bb0e745331b166ab2bdcc6ac" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/137.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "15cf5d63edb5229f7cd4d78dd917b1a94aabacd9018af4f25413fb732d2e0633b71fb6bb9f25416f21c1e4eeb1d187182ef8e9e25a064309780893f9cb73a337" },
                { "af", "8619f772ecb6f435bd45817ab2df1d1c9c6b645e8512cac7218a513584abb629f9ab947d1fbb5d4ba8f2c26c74b558e8e18df1e87227ee73bef54236e834d277" },
                { "an", "d3b70322c50262e1ee2cc2c161f78d7e47864791ebd3f1a2f6c2a7cfd443098948ea45d426b43fe75b73ea78cc5ff68671f61aeea259126eb937946244108d4f" },
                { "ar", "ade884e1bef9c2a8a9da990b3eb31f26c1b0900fec8e2474ff26d247274efc27635d3ab54631c89a0a86f4e02cfd14ee73af4ef5609b186b69bd1e800705f8ec" },
                { "ast", "0f074a5af3267e35ebe94be2d0658be221b01203bf7a67e4bcffdda5aa74273a3e12e9e34f53091c1ee16fa2b269ce0535e5c26894bcada0ce407f9081bb304a" },
                { "az", "ffbd01e15fdcf273100c845091e1eb252a86ed8b9468282798a81dd2cddfccad3af0f45ca99da172dec3ecc9b11b4183c561f972b457f101f6ec7e91fee793ef" },
                { "be", "f811ee05f1932b556900e0b6d28a7a9a213e6ca3d6ebde72cd167b584e632d991a0fccd548c04fd31b3ade8e7ce918bddd9e1e7624ba9727cff8ced60d92d548" },
                { "bg", "981475218efee816f8c8dacfd052b59561daf61ebd31b61f773df1a4a9fd20889057313460872c42d60d6617d54c27a48534777717add9a763147f5bf84a4f09" },
                { "bn", "4bd46ae059f0595ea4d5352808ab2194ee27a6e3d2b46ce98b1a66b25a552f42738ba0ab4e60494101cd36139efa6118fef12e1e0efa813fb19d765f72696a1e" },
                { "br", "e369dfa5b7f4c1a0563fb6eeaa7fc1eb74fa69293b8ad757c8bf8296577a98f6b14b62f0b40388e71871190ccceb4ef60f9ec14fe82fe80be9fde59e1c1bb656" },
                { "bs", "042a5e3bff54ed73d682607b6b4b61506abdcf3c43e20a61619cd16bdc565126c6864d0f40a2829ebcb461757b659245472dcc6d163c0cfcdaa33df479ef6baa" },
                { "ca", "f4b2b4e041a84821c0bde6e4d655acbb72afb4050b27d2a91c5cf765cf4a6b2f24e559d3e6041e279f10527e117dba21dc44974cec72289db97bd0bcc8bccc60" },
                { "cak", "e10c1ac41132a6d152355cd6dbe6c0e0b332857419a749115ff202700a74f4da07288e3393e903907f7335411ff13016289bfd3b8db0e9afe454628145b4fbe5" },
                { "cs", "9fe512bf8e05e254bded2e3a242773cc3098265ae057c120ca551639a83c9ad17d8a6d9c3183f339db83649aaa0b159804d8667d953b2a12b0c7aad9279f2cd9" },
                { "cy", "e63ed82cf8f424bc8ef7893ae2d1d80281490dc0f89ee23d07f899a3027a3c3092b4ca723a3f8922084f57011d26fd82bab314b6ad27f8e3aeececa1d242c7a1" },
                { "da", "415c94b2fb3bbc422ac75a34d05f9f0c7a157f09d4b16e4afa0aa49df9323d1086e0e3106069529b500c099837220d9effcc8e2990407e362b4406c5256bbecf" },
                { "de", "18a4e3c7c00d1fca13bc9a33dbd42ad7b965e8fce95f968870abdf0d3eab79f00be5a54753afad6cb4ba0b3b1da89e200fa3cfd24076f6077cd383a08768d364" },
                { "dsb", "ef64594b5a807cb4508376577c4b38ffc569d0573c5fff19f6f64978661bd1485b37b47c67efa331f8cc59512145a0c304ef5709daedb1b86deb5454fc1c9d7c" },
                { "el", "351306fff2bdc2b036e322136329a67de457ec4859b7cf3daf2439f396049fa3f9249f357d53687f13bee25e251b4c039bbf723460d73129f86697f1a3aa37d8" },
                { "en-CA", "4577b11ecf64749c30ce86c683603f9c352b1743c4fd26de706755e45e81fb5594a553f53d39870ff5ddef7004b624c1cb54ff89c46878d3d22f36940a6545c5" },
                { "en-GB", "8cd55922e1b4f5a541ea232e3316feed04bef250c52dcbe082c8b39a8c3248062e7df030d8e9702852c41631f6b9de7253da5090aad6218c03dc953e364fb3b8" },
                { "en-US", "1bf4241bcc1c41a516f6313243153851ddcc49c22cd23fb8d5371a4b06b839235924f77b206a41880f71f47be0636e9ff2c7873f99daba106c7ee311513bb2dc" },
                { "eo", "6ee9c4d466e02c2c173fe835fc39090f58d714ed9974f78ad7bce40c2578dbc74ff6624b71dc8e66cab0411574e79ab15db5c85ed6d874926627bc62b598e10c" },
                { "es-AR", "312dee06945913a26ba8b6c955988659d32e4b9a87f1fdd3f0a54535be3ae6bb18a6861187d8549fa931b01f2b0df3441d75396afc5957202ffc83af4a203da4" },
                { "es-CL", "390fe607674bc819ec3ba46e56e5de550fade59a23b6e66dd042e507e6c154d9c1b414d9f6fc45a8fa621d7a5fd545a7b2839bf2a03d33a5caa462230a3349cc" },
                { "es-ES", "fa783db2c7def1786e83a7a29eda24c6e3ae37c8037ccaf949b95a33ba1dcf280ff16a624558ec1746d3b2bd3e9c9810e92147cd5201d67f492793670211b0d6" },
                { "es-MX", "c00296e5a9a0ce0663c2b447e1675a6d542dbe417850e529e636f42847a31555e14b21d1352e59ac99774384414f30dba5513b544f3b015af8a7f080b7f262c6" },
                { "et", "5ead67a870cdd84c74850c4afc8a3df6a8c9d9ce97934a0ae25bcafb37aeacc328846d67b18ae0db0a8a6a8d660a3bb63eafbcc4506275340a0643a9468520f9" },
                { "eu", "c7d87096137ed1cef02604610b90fe5e29f2bce6ad4d28bb51104a6074391e54a9c371146eefa07e9dab9c13cd0080a560f1b9aff622a0daa2a4b10235df4588" },
                { "fa", "34743bde5b22b60628af507b67ab1960a19b8323088f20f414c8d429363d7b3ccac369bc5db53b1e0107d516cc0b24332737b7e4207d6e11c90584ad9d5e209b" },
                { "ff", "304f04f16f26b2b2423fbab5e79083c4619604524521f8cc26bc435b2ee19b7c608d27454f3d7f3fcea2a29c9d441576c8c736e8b7fa05704636a315e8c23f38" },
                { "fi", "1ae82a25c01156e30235d1e2787556b8cc037e381c1f6db087b435cc87710dd7699ec9024180e07f14bc7139943c149515b5b9890c3ad5f723c6409b76061ace" },
                { "fr", "4f0412a25e271c50f15f247e212b35e0cf803e4ca355a3a3eeedf3c26a683cbc265653b9a5fa2c8fccfd4c227ffb88ae6f4318d0c23a470c22e43cab265384b6" },
                { "fur", "d2db1cb53395cd649605d7db5e3ed1f21af7a9934ac053537123fc4015bdb7eab03e2623eb1cb3f78a210c56cf28543c9663482ff022c40a3a6455db5d4bc515" },
                { "fy-NL", "33069fe45b9b07207dd4995cf3bbc04e9afada660be660e5d9c0665e1a493965b239f82806db93fc96884aac8341a243a0e57f1672277b2a3f4b526dec13b5eb" },
                { "ga-IE", "2121df62490b18eb951582e2a1bb88f53831436e14ccee27fbf4c2d2be1ebd183df03d4b92a740b303fd7423134d75ac8a87d0c27387af741b463aa8403b8c95" },
                { "gd", "189b8fc329bbe3ba9c6613e1d4f21cc8e9c0edf37bc62445018a0a791b2ebf1eb8d282b0f6d856c0c9a12f948ef48843cf61baea5a333e03ffd28190be2b69d8" },
                { "gl", "4d653c2b4e598e604b44550fa0b3adba24402fab5860e9d4df0476976c1dff8639bfad79e42c4c44fdd07b0422f453821bf3859ac5221259bc0b6d16fd7cf37b" },
                { "gn", "72617a97c940387875513e297699d10d0e0e8cf7b54d370dc31b5d7223454c60c106a5d6422dde07b8e2213a8b33f073a7d122c7d60a2e6b36ffe2a61ea3da34" },
                { "gu-IN", "2db2a7f4565ed180e3030f2550d8d3d10ace959c9583b940f547814f73bff8041f966e3fb591825d61a1187ab759707f253da102a8cf316fda12f26091904f17" },
                { "he", "2aecb76696d910fa287e4624af88538ef89beec9d17ecaab563fe503eedcbbdd925397fd6a6dd88f5f9b90f0ccd85b4a6834bc8e8b834e461babe326cc568883" },
                { "hi-IN", "24222e9db7ed88d828008ab2b492bc283123fd4829f78c4f6aafe245a197eea34f8747fb5041a08532f3d65c52700d9b885ef76ad3e7b111cef8762500ff2ba8" },
                { "hr", "3543295cf12bbf73f66d2fc51e92de1631479d06636017074dd265f4870a16b81368fe6ef63f4d2297bbb3c8dfc24d74208ed608503b74e1d827faa1217f072b" },
                { "hsb", "a696eeeccf033ea364734782290875654425878ab44111742d65423ff9d5b02744becab17ed6ba085cd867c8bd7f7d84d31bee3d440c09c6082d994060004f26" },
                { "hu", "46086368cab05d94ad8abe94aa586a5d4bd239c4680cf99e07ef5385b5b818f27fb892d71e79de281f39c46dbcbe66472fdb7ddda4b2e6569cbeb499a927ae80" },
                { "hy-AM", "7fadbba96a4875fcb526bfb6bc2f417dcc90e819d0aa1bf5a9e9cb50a75bf2b8f3ea39641ee47586235703ccaaf5f4c74918f66ff4b505615c24082b88adae80" },
                { "ia", "54f77e2aaaa9b0f2c51192c16852f76524e3adea240b5f96cc72de96dbade2860579a855ab51995e8fa205c187dff583e26fee567ada9a89ffd15196b4d3bd64" },
                { "id", "eafd83f1ef2dd2c8cea729bb2f2128a602cc486b3670e1925d83fb890941e90e18ae9a2cf8b430cc8e1080a50f1a8a5fb5f3ebbee796f25fc6e9b6d83d3c4d52" },
                { "is", "c3018a2044c2b674256b03ff52de2c9231b144a2bda75e3603924c0973e912f6997567de87b53f0533b8894697711ba62bbd35b3530f5a4d4225a76187aecd3f" },
                { "it", "1faa7055434dafdcdb7dc754e0edfe799b74c1b6b2de1c33d7add16b9797fd402049c50d67d7df633916253aafc90204b25739d6598892e02141398f92146799" },
                { "ja", "25fd23ce1b954e2a45630fd1f41945f593a04c1bad24be1caec36493faa81c6aeaf1b403f734333bdd68f157f03314d66020567a5ea80401eb80e54e3a8ff576" },
                { "ka", "9a34c01aaf1b314d666dc3a627c9bb2709e7bc8367ebf30c28072ace81ef0f8a9833a4daea155264ac0c3992f7e8cce60c3c622c388fc619b0a1e15c83da563f" },
                { "kab", "80f1c2286f77254d1d841f72d8e3ab361580dcbb3855525361f756e8d89999e88029268f0f009561c9acfbaf1112a79be48095a32ca9c7f11d2e779bd977fd94" },
                { "kk", "911a9ae7f5f12dc2de930e8f5edb0763f3deefbe627a9442e2f2fb0acc52fc5bd8b0ca7ae260fdfe08c420c69940953ef2fb3f8fb80fbc7b9478f7d7eef380f0" },
                { "km", "93f23ddd2481f290bab6dd191787ebe4c214ad96bdb5708935350fd1b342c4f433cab74d6e1f28572e0abf29d764ef5c18782ecb171065fe2ecbfe026e80fa57" },
                { "kn", "3ee5d5c2a9ca1f39ab0a0946527e780331e4bad127657d74d26d6a633f457dfef9faf48659994e2d6c7e4b9fdfcb275a3ded3de663586481d72eab8a499eeaa7" },
                { "ko", "18f3693a1543eb59bd75cbc2eb2ca1c7b1ef4d2215a741e45f5a082a81bda6bca9d89b6a7cc63e59096d7ab84a2f553dc7602fbcd01fcbeb2015ffa082482f9b" },
                { "lij", "1c741de7e53ad796034848ec52cedf3a1e9fb1d169695cc0a23f28f7ad0819716d07fa05ddc8ef3e4d924458c6687e13acd060f37346accec2fa731e8efc9ab2" },
                { "lt", "3211d09540158318ad47867f458896c517dc52c544a721688f1dd461766844e1b1211f8f495d54cc9c368a090f00d9853d46835d2aef8a8b2e0f291a4fb51632" },
                { "lv", "3eb07fdadc3950cefd840905bbd73c54a8a7a4eb3ad90c573299922f34dd5c409ab6f6cc69bb00d26ab9a8a2501c3ffa964afb999f9ac18be2bf8570af218efb" },
                { "mk", "a66e21c28c7a03d5663454c732e727fe56fe5a6aa66ee9000e9a90d38b342fdbdfdc1a373f6771b8660534573f6c46c2bd6687183369249508539496d7d5bdd5" },
                { "mr", "4153a72ce86c2fe21e1a775cceba7d881ac258308e86b52a13d3feefb1cac62d69049c962d29c9f69b3a1992699236cb34c90634cd40268be7933923f3c54781" },
                { "ms", "8eaba40bcd09fc3411988fe9065b9478bf5375eceff59c1cbc7140495929751e5bd1023502b6cd1ba15a621b4743d3589e3fbfb017e1783acd99b8e8c5d4ed39" },
                { "my", "925880d76ca5ca4247f0800ce6dbaac584f1263477f4548106516d6829a4b619138f62080924241b02c31962b2e12a8dade47a20888152d14797c323a6ea7f31" },
                { "nb-NO", "c55549ef8a1e9aa61799f31a9fc022972ff6736d6463dfdf80c22becfa252c773462f3d3a68091d5878afeb996689bccef34af3d8dd390bde91ff7bfbf4b34ea" },
                { "ne-NP", "5a70d47373e96bb85f6fe2bad3b6ec1b30017a68b0a891efb08d38c5818df7b349b56657b1d124766e0ca4b3bcdbcd60512fdd2a4f2b55e6aa44409b788f6ea6" },
                { "nl", "c6e8edf6c0b12e4c0c0bb3566e775302568a6c09efcebd155e3700b937723a214903e478a99c33c8e7706fcabf5b403508cd9ead4dcec1dbfb4493e478ca6128" },
                { "nn-NO", "cd9d6142345804c4e3c3edc1c5f0cff9ac1c38984dc440a54a94a5a3d22fa0b9c1eb2d908fa19799cee845b8a30e92deff8283885d593ecb7e5aec5501d801d8" },
                { "oc", "5e1219b821de8725235e3098e13310462c090445a7f3a7ddf768c5720e1daf4c39c6d05d6a57c3c431527ac594e88c2b263e358dc3ebc6e42598804ab7b9c826" },
                { "pa-IN", "fba3c8aa32fbfde9dce9383c6da16619ee1f22e446554a19d4521e93a54eb21eae02005c3c567b494a05a561c680415d3669d48a82e518a33d86ebd588e20953" },
                { "pl", "df04a2b39548dc79cc94a33159225cf93f91d1382f1a2aae8acc649c154f4dffda8c9d1210f20466bf848b3d74a47614768cf3fd1792407d12ce82201dc63de0" },
                { "pt-BR", "b950632b009bcfc35f0c51fd4dafcaae0f258ee1ba5159300dd7003662767d5c3a2838f67e6bcfe617d08d4c707849a6e7a7f6a39c745e7fc3225f5f62711beb" },
                { "pt-PT", "9db26a1adea72e1da1cbc7c3001923ad0e2f8844dc221ce313e7292bd6a237f8622069d7bd6d6a6dd57b9e32eed53d68099c00105c17bb45ca0a6361c86cc2d3" },
                { "rm", "c4486efd9417a58c48feb7e27a26d9d3eaf8e5af80016f7ce83e4ba6ac695d69471065fa895d9336df2bffa423a60b7b589ee83f8a1e51a5d843e3e6d90911c4" },
                { "ro", "2b99a656a4a7f1baf9b6995b18895ef2eea488c1883c754410225d763ef3c8f73fb2b9f253968df0440e12011bddd35556a9d3f3780ca33fb7226dcaf2f39666" },
                { "ru", "0a4daadd8fc06da81e11980949b599d8627049dbf804ac8b134eef401dcf4e0acadf5227294068817472b7f30ccc94986a671397edef4d7387c8bd4ffa854815" },
                { "sat", "b9e1739bc6a55b04b5572f13c761388114405e0eec6c6d9ffa56cc05b427c96ec7eb2b18028bc37db8f7343fb66263b2cc0b77ef2278e9ab7a26b08e53bd6752" },
                { "sc", "f9e7f4f948f06aed9a46c3609652e6a1a86149c443cd48c9d249138e3f00a806060743068a95f8c9a5eb4cfc67d80b63a8c7c80dac4e425049bb9880965bcfe4" },
                { "sco", "89d83dd72c7a94645dfc7c1656251ba2b8f804901d41a6a41e097c0492b25c85425f23e348d09374c51225d11f913b36f61c7414d4ed51e797cbf3d9922c1825" },
                { "si", "92668468f2061dd4a77ec35c6a25aa1357b16bf315445ad83c3e939b3fc87f932a6d6c3008acd83bd3005abee44156ed84088f203abb3de8fff03a97c56f19b5" },
                { "sk", "50dc7a459899a5a3d07d21d8deecdd453c27f5bfce3ca7e9612b37383c9559b377b7056419b95539778b4a346179161005dafddfcdbccde73845c198ab8a1699" },
                { "skr", "044cb637324031192856d5592b296ecd61d3a7c6db91bc21533aad0542a59593751e16dadb130f5fce2d4f063fda40cb8308a7c7871e2abc4cfeaadc74e936aa" },
                { "sl", "2489dedc9e0d15635c8ef832e2c82993b58931ff6b9ba12d29a0036636096e91d3ccfdcd505d67aaacd1b6e2472122544e560eadda3bdf7649ef5e899ef3f7b8" },
                { "son", "bec8949c2a1f805931e56215a19272c7e8d3e156166748568903246d3bf789c5e66696d1dd0eba8e1235123f003b8ffee8571e339e77ad075016d7c73bebf3c0" },
                { "sq", "fc50639d9ac76f65333e52de1dbc3dace036a4742887e1a334b0e54507d517266630640f2cc0be9bba159c34af0de9facace8ed4bbed4649b40b13ce1df1b1e7" },
                { "sr", "72f22b84a8e014dee128a040908a31fa09a49a3e6b1c957fc97023abbebc088ddf75f446464ebe23f895c4e2531df02411ab7534cee88cd3540c079be91d800f" },
                { "sv-SE", "36d034de56bf10675e2bf1195800247b9136314b604bc0056e858493573ae18fcd4ce108cb146d9759d670d615fab793e4364f5124293e4c9207f4d976fdc234" },
                { "szl", "6d1297f376966390159c8c690346fb89826328af8319bfb26387a1f00f0adcc8a170035d703095abc76de2fe504bcb83033bce0e9b9b87595947394ec5789daf" },
                { "ta", "c86191f508128e5fc3a1a34b6cd77dd7f7a5ae508889fd396210e395590518a191ff29cf58729e36f36c8e6e07c9e29d7a372b8a2e385ff3f040dcc1a4cdfe53" },
                { "te", "051e665398a7f0631440d81a3abed6622e3514950c223d0649a03adef11c3af56cdcb1cdfcda09b6f08dbcdc8207b994a20c06435dd5a3de98c531a295836254" },
                { "tg", "95dd247abcd8b6f5783b4445c921e1c08af3aa7a5a92580b66b0df42f7db1465e4f1778fdb8337bc4af7c3f9cc9ba57b118d62e36ed761e19727f1da44a69e74" },
                { "th", "b4601d2a02f42db475c9029bdbb85a46aaa5b94503a3120728ffe8a2fae5f432bde9c4b3c37544898fbda544c0a3e2fc78c37beca68cbcae85b6051049c786a0" },
                { "tl", "b342ce0dca803471d6477a127c67789089e2292c50e036ace8809f6ce3b7c05a87d678e257784abdbe2510bbb8cd8acfc717aee14c9523ab337e049eb2b6b754" },
                { "tr", "c5aeeec1bd4bccb64063a693f052cf3fad591b8c38c7fd302d9a17a372892f785c00b20c14ff42b3c63ab84079214b234beee5059fd3178b6e8ae7a21b49c3ed" },
                { "trs", "beda37a9de55e0d22c32a2e06784ef1b7f03bf52479e01bf5dd296077c277215290ba0c25d1e00167a178d8291f9333c47668a69db7e5fb482722c15f819aff7" },
                { "uk", "bc4ad86e8992f50badb4f1fdaeef04913ea05e88750f2553a61cf6c6c1ce6a37342d58f9bc7b0d37f6682f3e7cb23265d5dd158d27b3482b60027e1e6c4730f7" },
                { "ur", "c712a522d9b3a07a54d5d3345025d178851e118549591e70b5cdab1b3fc59f2ee69e758609c75d728299dac332983a20a27067f7dee3a8bf8fe94964e98fc026" },
                { "uz", "db80abfc7c47c3c91a0f64236eb706a14eaafc265dc18f805c072651a080dbb17cc85c1381d921de86ba10a070b0c72b0ab1e3f30598ea700f900e57bfb442a0" },
                { "vi", "35d6e8f93c5342903b34c170418aba1e14b8fbdc7002591419fa22686ccee08441ff6b1ffc6349c365ea397e056d1243938f4e8ce7004a6fe2975d4c0681de58" },
                { "xh", "faa6e148d17fc444a48f1abdf57d61f6a138b0b5f275f94268b6025ad088ee5ea1d586c3e15b2407881f40e39ef399fac59bbc92fcfb2008b510bd82cf2ac1b3" },
                { "zh-CN", "10c2559df7cf20ebe745f75a93caaae66cef19957bfb7aeaa5c2c34480d29739cfa8dee04a675e71986763fba6a690b4b0a1eaa263ae465a4e839781b7ac4f67" },
                { "zh-TW", "fbb924266e931ac5762394b3c24a96f0709eded1d0130cc85d3226761c3c9f094e60a9c96cdd28ca0e6bcd24cf4913bc949e4baadb899537c4fa223c8917a984" }
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
            const string knownVersion = "137.0.2";
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
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
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
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
            return [];
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
