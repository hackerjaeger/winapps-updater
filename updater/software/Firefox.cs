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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/125.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "7656c73ad1816fcb013667e5858ff159622843ea6b1ffece29ae18e4c22d6b57b4de98bf0a77f71379d456246ffe4297dc83343dc7273a7b053d739a3faefab9" },
                { "af", "6e6e85bf73cd6bebb0b9077dc1bbf0788ab600085f7512b3a599e20abbe05fb07beb5b5aca57eec51fab27e2f79b1b7d2b64f6a4fb25a1c67e0e601aaa90e718" },
                { "an", "8938d2c8ff43740d4953403ca3042a7822dfcffd5829883312a1549290c61903d18e7591076f5d4b03e0bf4d78368865d13de1c698794efd3e41220beb9cc195" },
                { "ar", "49ffb9784d5068a85d69bc7c3cdb8df7359780d469c72c9dc9c336174eb956b4902fde8f5452b53ecde7632596cd99c94b452ce282d730d1876fd6b62028e492" },
                { "ast", "89991b966e7ac71f00f7dd3569ffe7390d53f46141c53cb237d013776aa7e972425d2a2600bbd80e6a24f4fb8b05dcb3c5aa1aaece181dcd81c50e7ad38c1495" },
                { "az", "dfb21c986a321d450dc1708126adc960b0bac62f21c6df411b6ddfa7f409959703b6d8234914459735d6ff14e7c0ae0181b3e145d08e56de9ecebcbba8f64446" },
                { "be", "bafd61b8ac0050b8d459ad9a008944864b0323f4b48897290e11fb12e4f4a68e8b5bbfa5cb914356f299b09cd3bf1c58f06961f0e9c6f682efbc72ab9b0f5447" },
                { "bg", "bbe908e5f63a47399e371361f9a70ee4fbaf1f1b9cede003455ad3206da6b94994c220b21c16f00a5c1858c6e7e2b5f86c9c7cd8fdae0ede0796c7f2bfc51301" },
                { "bn", "b04325a5218b8be5b0cc472e30ae2826c59cebc6e3c686d66ba067309c6339ff4a4feca8dc11247f32db3a9da73d876c1239105234abe21f7c651a6bd8fa6800" },
                { "br", "13bafa23c9ae79eec174b58fe798ba6505257606939ec6fff40678b31d8fb4259342ce932f186222eabe0c00e7c86df0dae2f2045356a8b6948d04eff724e61a" },
                { "bs", "1b8355893313c41ae28dacaa4c85e2d4e26a088dfce3f076e2463685a5dcfda3959022bf16c6272ef88b1be31c3f7ec6cd8a4e3016f27c31a184f45312d326a2" },
                { "ca", "e4b9f3535d4c53d5e4d528f9991a091c7c032c6204af787436569dd49934b412094a18637d8b9be86a71740ad25935d0e4ceb20764f188574156ed511775803d" },
                { "cak", "7c8d7db84ef913d746ba8e330d35619407aa1b9ecbd312157e62634a1eb4a55c4d5db9bd9844749c439ee8a5e623ff473b875f956816a932083c319b8c872cdf" },
                { "cs", "f1cda39100f556e24c22e5dd3a06e7f15b3261504ffcf20fe65f1b708407da880d6daaef81077687bbe821da9b68b5f34cf508bdd7b74799d06b8ac2b0522f49" },
                { "cy", "3940c1fc11eedd97de508f42de2fea5cacaac00e1575b164e15f1ea68ab577f7203543ab07330717a2916746bbf470161798cbf4fa0b9b948e60448b94b9ed96" },
                { "da", "8d6c21f6c8ba2066b20f1d6c4f29b1efbcb98a2c141b4e4f79fde5e2402f45a355e139a6900e0bc84185a1fb7ea7af564f7821874203be3d8052bf24a9c23e49" },
                { "de", "8d7a07618ba5d6d083bacd24859948367e0e89306f79526072d55179ecb8c7b1beb3aeba68c847b49614b4207f2ae4becdbf02bfae50fb2485b86b389e11eb99" },
                { "dsb", "c69bc431fa440544da9883c6c28cb121f76486706e284e2a0ff8cb619afea4b5b74c8ff1cd0b5f9f306dfd7faaeaaef2598652277dc90b9ce069a25fdfddb2ce" },
                { "el", "1e72f6f0c2cc73d58c06fc38f89151465242be1779e09ef72f2f67289026cd4eba3fdf3637d1bf6e51ee4bcc1324a2569943fdbd9e3e3f226fd8dc17cf99920e" },
                { "en-CA", "d420f285697870b0904667c27f90f820f4ced3f504744a1a51c4823933d795487cc165bc37bfa0e0ec348949bf013f1f629175203a799f4bc6f8aba3d52d46b4" },
                { "en-GB", "b62d61eb1135b55340140096f1acad4c65c8e021a18c6a7be7881d29cbc7487d98085c0c76284ae45d08aaf0f7946bf313258472557a05bc253b9b54c396eadb" },
                { "en-US", "09c009176ba363b2e3aed9f20d8d4dcb41d98009df58deabc7e3140c08498f9f232e376cdeb6246a6acfcc364f8c96d50a0da69f6510de0d453683a56aabcfdf" },
                { "eo", "e8261c778d6ef5a88d45a6c8dbde9e0a55f30e0313b228e6d51c0e8e4d93695216e3676c41a57373479f73a829cfcacab812093c38509f1b78ca3391fc2bbe8e" },
                { "es-AR", "23468698a0324aae8c8ce730f86bf67dc8a3c8bd210e8b600259f19b92a4882d0dabfeb160c1b3ba1cc079ad298911267a789848d5beb0887d7d91e0f7f9943e" },
                { "es-CL", "079bcd7f119c05f003cbab21b151193ceb35087bc8c315f3e6e87d7036d90d89f4b56577634faf5a30c02fe3f44a5dbde114062e64b0de3448b7dc93a1d12b68" },
                { "es-ES", "7208110161ddaf3d520fcee0c7efd6727ee605aa529fae4c0d9a69d551266e12be538ad6ebbd64f1197422796f9989e9e4f1ce8813649eb24b0aaffed65467c8" },
                { "es-MX", "8bc30e2ad56a5f4edfb5b231cd905ba07d943d5f46e88b06899cfe70b3d0d666ab0f3b8659ff9c821f2cb3daf1c5b844e055b6a2ac11962658a06db50ad3a561" },
                { "et", "cd7bb7ddd1e287cbd564bc4e2c3bed6f47f5691f805b2b8ee1d230917982c70cdb5d89c1354d19898c5dce3e8104fb86f489d4919c0b621f6e64d50184333e49" },
                { "eu", "cb16f960d6d1825bde90e009d1de217397723717ad03b5d7e82b62e27c5a50123ac2efd9504872a908355c8e151db22e7bc489350d1e5099445bce4dd6a8ab59" },
                { "fa", "9caaf0f82965f4735e3bb51a3013f62c86f381dcddaa6e0d1f7b3c0dd8bb3c962788db03c7c2da4588f8a9c92d8af6ff265e733bce7cc44d852d71c44fba46c5" },
                { "ff", "793847e687413d2f8baea10a685614b5497eef510e7b6d1b1ea474bc46e13c6652bcb6ac13847393df6001023688e0fafc7c2557392f1d591163c82dd127d954" },
                { "fi", "723b3864da8612d8600fabf27c27f4ac8766e4e361965c260e351864b3c7e933bdfd566bb8815896a4dfa0bbbf38dd56c5feb07e452fb390c6fa9489243a9acd" },
                { "fr", "b8b5d74077eb66e4f0b72cdce563d252f59ff55823ed99658324f799637591526b0b1d9144471e8d1eed93ddbd1bc88b4ca74a2753caefa98cf569eb4582dbbf" },
                { "fur", "79008d4c4a672dab87c50caeb736d978d66dc8d70234cdd657651d25f2830784ecb04d2e94cfa450f2ec7217cb4e33dea740fce7679b32620fcc6f4d8e965f70" },
                { "fy-NL", "56cbf4c021a51ef8d311d736488e743ed86ddfcd9a824d782656c54b87edbd34efe5487f3c2551fc3239acdeafedf72bd13a33b8acb379e633fb3d6c362e008a" },
                { "ga-IE", "6c361de2b7a6f68b8974367280717d4096f903702c5b18a94bf365344363da1ccd0f83203465ff036507e1cafd54aed6a7f1b0f0afbccccd6898c08ed6bf2cab" },
                { "gd", "cc17388bd76cf802c9e12bf49e980f366a2a8000c73fe82c81b4cee5e9f1d4067ec1c5fdb478be7a6c823a62e4eb6acd4655dfa75df146b1b68e9c606f35a608" },
                { "gl", "f87695ad34d004dcc18a9d86f4e01ab5f2978746bd1c323212e88bb61060f39e6535fff3f4a8bd199b3a27bfd3d18862f31d37bba69a6739114a1229c40063af" },
                { "gn", "d484405456b4ae3410ecf4831c33729998c8f8732fd2ebe078a3def72c0478b45c80b06c3570f9e4bf347dcfe5e958b9a26c293f707ed490dee2a3d5ee8ca2fb" },
                { "gu-IN", "4f48f5e75fa9ae782e232fa8b7f5997821574e1ae84fe1ab6f5afecb965b9fee1f595377962b1d7835860b38d589d383c2029ece92b55f4ed34098a8b00185bb" },
                { "he", "c1a6f0b5a92350447cef84afcfada5847ef07b80b1c3ee9bc5db4379be7baf3eb9d1614b9d805645b01d6ef1b3619abaf0b50bea7300e53442e2d1cf31664643" },
                { "hi-IN", "fcc970c58e50e96a5be62ad9bb6c60de0e72a89a96cdd641525115c5414c071afc66072007ec29680847ab6345380d297e185bc80d53c89eac4ecad39df02e75" },
                { "hr", "5cbbe3c30c4c4697470eef786f81fc7ca770380f34588fa3ee2ccd3a6f9b541144c32a97eacffa28e60656f76acdf40a94da627afce94b0cbc7df61b5bf4a0c7" },
                { "hsb", "2bc43416ed104c4cb05eea4002fe02744f410fd0cca41e1405becd604d2637460a9cd42b90503525a5c84e9b2709f5b006e689548d068b9952cc6023a7e7542f" },
                { "hu", "51ec34fbd37a07e3a9e7f2bead56c5ca9d22093faf908ae8ff2b6412c5c7d335f57d317b3278411b71cb2dee39ea5aea24ab65d5c18e0c221da29362413a1b61" },
                { "hy-AM", "44c7db19e4e29b4e798406dfe11c59d9ce97f889cb094c8c50fff256a7f515b24643d8ea7d25ecbca7fae46e7d4c20f9974e9ed7f16fd8cb5c2197ced2ed472c" },
                { "ia", "e6069a9bae761a1b234db1069ce100d82d005bb7200ec4339d2ba4e75cd0eb6675c4339f93c9ef122fdd75d367cc66f3407999503f1c00ad04a9f1c940f53979" },
                { "id", "20988f306e2f1fa59fff22c47b86c158701166e1b68903c7f5296a8eefed7455d072b85a54362eaf96c6ae7706b16a1d97e0ff7501c2c61de69ecd7b3b1c9d14" },
                { "is", "6498c05ed232631db48d3dfcd9e2d996b5b0eb09fad24d2e1bc14917d30f749c7b562a61ab4fc710b9c9aac1d242e7904e9e59fc753f6febad2589e71d8b4639" },
                { "it", "b89cb922e69640417e13dfc6a050221a62300a127d16be688734b8eccb04e63383586decd1b7ebe0b4eeaf00dc0fd515576572fd57f2dbb5a06f5d49ed0ef59b" },
                { "ja", "2ed56c83b1a329691d577d4cfbba0708c9a5a61ff6a4f17a45e5191c5833a7d768dfaa0b44fabff07f4871e2f5d5d1feca8ca609141ea6a72dc54463a5726851" },
                { "ka", "4a38bf117c0b1c54f5109a387572c65139c0493355b41cad19b66c2fd214ba8dee3af3bbd0ec8c89aa88b94206b8c0672d54e5a0ce41af0edb6eb1b114813e5e" },
                { "kab", "ede1462562741ad531d946592cf6d6daa82e5aa7446c52541aaa227c3dcfb1db6e1d52f9e9d3b8e230e4dba4e498b1dd8f5f4f471513c41d9036563b02ebc127" },
                { "kk", "7828a897d88ff8720640418ecf758bec567a958763d9ce1652670912281cabd41fa65afe31ba3e824f21d631b28cb5123edbfae6c0c991679db9b70e2d3b4862" },
                { "km", "de5c15b3a756b21d4579065071cdf90c5d62e9384446cc602b23839d8024b9af20997143913cd292cb9cd664e2822f406a8d083ef1df9f424c51a0716c7fc1d4" },
                { "kn", "4fbd14490a3f94088e103a85bb75d22ddbf6a0837515889830ef07a942baf60465469f9f1b5292d9bfd32f4c9838a84238bfaad20d3bbe0090e86c9db0047dee" },
                { "ko", "591d810b638f88242dafded30c443a4d9e065355a3ad0aed71e39a6140fd7b897694c615b9486e1e078f5e8eb8f1a276c8c13492fbbd76a307bc0efa6f3dd711" },
                { "lij", "2d352390e2f37ab06e568819714920601c10f818aca2a298b2f2965be36d8f9336e8cb0ee60e9779e6570d49f903f5e98e636a151a27e9a7b9ecbbbb69ff9dc2" },
                { "lt", "ad46ab366405bfdb8f45a11029cb7df92b1d56cb6915e374bfdc86541c468f4d7ac3c6271bdc5a0e7149c5f98963b4d43d5c19d7a58b61011d08d3e13f6801bb" },
                { "lv", "63f4de99ce39df746061e4b7460f52d7bd3b357d4513476247f6b0dbd935e4007e1f761aa18cd5e0d640ea23e6e713d5abf2c6534b78c6bfb3acfffe42e1ed6d" },
                { "mk", "44091e690301846f0a63a366f9f2d052d3636c91cf692a082c55da1af67914c5d2989e071744b776a4dee9b6d48ff2e9c49da8d66c0c5942c92f48684a6e6dd5" },
                { "mr", "640d4699de02c97667c428c9c842140e49b2825c47fb0f1782b65a6e6e82d73393e0dfd7a514306e68870c01cb0b3e618a62b1d3175ee2c3d11bfabde78815a8" },
                { "ms", "c58b1059125b317b6048da5ff945aed64dea3ebce310ba763936c871e7a3268739d5c37027f18bd3ed554a5511f48ecba2b284e59904a213bff99c16f3af0415" },
                { "my", "4ad7dd5fa1af1ca2689076caa7756a7dbcda681aa08cad91b87a04c432658c56888c61072d6a74c1a996b59a9ab1904388c8bf3762a15bc7cae8a4fc3242198e" },
                { "nb-NO", "cc964b9f83af609d55953138505522a526acecae05917bc0cc683b76a68011a9002f7fafdcee4c93b81c2e33fc4fd299d8afd23b57de5486f67f074d19adee0d" },
                { "ne-NP", "0c3a17cf5813bd57cc9cec5f3beeea0eae300dc62dc6074e73277add65e548b23c11c80c6bbaf095e341f6d99883765afe2aa1bd97de84333f6b12a7ed828c88" },
                { "nl", "262b7fb5bd92621af78ec856fc5b8ef6d0d632a761988cf5da4fc53efcdd85f05bff9572b97eaf926aea135edb5fa0a5fe476e85359e2bb6fd517e4f2d91fe17" },
                { "nn-NO", "de8ddee28b9c555e348d5674e7ae21037cc34664f59e32a506da6ec9eedd8b329ff6d7bf6c2fb652190ee4f593dceb173d4e92dee365f44272fcdabae9413ae5" },
                { "oc", "1f47da69f3cc2f0b350b5e8c88d828a7f1c1545e5dea571e4cede11ef80eb65de3e89436bb39030df57a9e380f68728f9d3bb6c7d7241b3fa3ec7812e3409cea" },
                { "pa-IN", "3ec88f261175d437b6768866af687ff66fbaf5baf7ce97e98bef82dbeef72b9b58ba643197d3b1b1097919baafdc425a5aa198e94c7324d7677589f47eabbc3d" },
                { "pl", "4889bffa25bed6a4d3920ce819383346946d1a02e03408633cfba1d30780e2128f825f53615853e696491e11009b2cbbcb60c09be45ebc600c83e1222d93ebdd" },
                { "pt-BR", "da4e93e6c3097a9dead36dca9748b0009323e46f41a9e4e5ff25ffffb171452e09cd93aa8d75829bcde0cae0c4bd3729ddff0b69a6fada550c4e966183e0553a" },
                { "pt-PT", "eb0019671a7b772b4281a584362665f8cf12c2d9727c1109b7aa4ac1b2f14001c50bd1cfea1ddc372d63ac32e7690206cd4901f7e905e9220d80a1cc923711b0" },
                { "rm", "f7922bf5ed78c4c4b9d7586ba27967dccc921fc413b9d53a7e6ccaaaa423c0c2addcd25b486fb01237125c5185fd5ebee6a6693381ec82142d7ad34d48833651" },
                { "ro", "d2df0e829bcf8d58fda29604da94b42f12ee2a6d4da184e4385b183106f8d1621d3b91423ac6b49d40ed0b4d13bada1356e5c0cc6550335209ef8e5e3820dea2" },
                { "ru", "e8b4eef459527ce5bb541c51458c957bc2c4e13e915ea3088a6f408165575a5fa25e949e19f337bd2affdc14796421ac77ea828eda63148ed0e8b3de25647423" },
                { "sat", "4d6b43fa267188fce7bc2b6b2c4e6bb53911be9e058ecb4077bdf5073b85233013410dc07ff94713d9158ea6f16efd4a5129b397c55558bbf278559bb63cf9cb" },
                { "sc", "a270b19dc49502923c56c8cd131841f32a1521158dbc0110a881e795f9a3684a51e1a943830914e279f47642b9a5dca0f41575f4b98dfa6962e582d222f1defe" },
                { "sco", "65956cf6f1c49c5abc07f67ba468e745999b23c5f12063d4019dba30b01e38f75e9ff223660ed90eb96fd91c7c054b7855ad358bbc381413b224208117bd6378" },
                { "si", "15a9925e2c16d6aa97f12d2d9d9e99ddaec6f50085fd6961e7db6b167e751faa64756d0b47fe5c386d1c33449bb95d3aa74d6d9ea9e4daf363d9d16ce7463a4e" },
                { "sk", "313912005ef61569e97bb6f61e17e76e82ce32de5a6942f9609ecf525d4b17414bbc29cecc751817b12e8724339392d0710d9b40570713eff78b2497f0063d58" },
                { "sl", "0c881c16ea88b2a739089a0848aba55f1f8b3be34d58619d25b4d9cd1989835248316bf21c625607cf872ca61be755e8ca7b21e91e77d681be02b9a54a3c76d3" },
                { "son", "90a1aa27ef62231a324ac01bee319488e50aede9a7c8387eee9b6a6f1e06096920f62427e6c260dfd23ecb15331426ba3f45167990de431fd673e8840a029028" },
                { "sq", "7c5488ddcbeae6132c1486e22fdd427b84f0f133d6a5292bc4660764c98ec4ee95c07d2d0f8c87acf7511d7b6cd29ceb93eeae8e3b3bdb42ea094ffc96bb1b93" },
                { "sr", "86a25ffa80757887769834683f4302174eff5efc537e6ea06cf8ca9c859bfa477d8b596ce68749d14c59620cf5d620724ffa81f6c47e9e18263d72cc9aa7cf78" },
                { "sv-SE", "c04daa20a683a0e5e5cb5e20845b3d8307829155e76ad19743444b927cc78084232ef771fd6c3cb831392f99f411f0b5994177601f88653824395bcf42c6a447" },
                { "szl", "b23c4e19f69da94fc51ef067625e0eb4726147238f6a64f692d2dfd7c543d1b07cdf0c070adc5e3e99a1f0d9ea96a811909e02db69e6038445d3089513aa7302" },
                { "ta", "787e3a74665a57e6d2c5e77acef6a4c52efafbad7d1e8151d603faee831f4efb223f05f55832b45a8fb2c418c69f2c05f0222f9b5cdaae2394df1dbaaf0edad2" },
                { "te", "42c485e44497aad2042b2b7e175d72d4cb55c83c8b3b8ead979c5617c7c802cbac69db9d9b653568217c4c871ad8e96d0df38056c43825c7cf35742bad90a1b3" },
                { "tg", "f8fe87bbab22ad89db8fff0fc172f4aa9f9354c0c68d3d3de6c1022689316b8951658cb94f70fbfc442efc1d110154bd941406394d8445be3e98783d3c2d0ba5" },
                { "th", "62b122aec3ddec86a8a1d52626590e4d835d357bc4df0cdeb3e5a12bcc74ab485af65ec841fedf101b7bd6951100b06795954cecf7f644898029694b8c253ac1" },
                { "tl", "a4328d5480443df67533b8208bbaf0c73878e20e4f3414c73b4db90cd98ee3faf1c92c728f9a5968072aa7b126fc688d8a3626d1b2c34f7f2c19b9e107a5cbb1" },
                { "tr", "fec97181ca380744063c5ac2c9d9f409143e4f76b9f494cf14f230860796dd7fb3010cb481df250f6822f348c3bd5fb1bf3a6c9609193318dc5f6ee75012215f" },
                { "trs", "be3197ccbf30d20db270dfe33f5cdab8efd90ce3a79e7a46dd13cb4859ccafd175ba8d434beb7f066741c5b03cda6309b80bf7e2b7f4156401cf69828032b93e" },
                { "uk", "dbe3881c555e834e2d61992ca972059f706f37d9a4a00f5c86407d58b443f4d2e664e6d8d63ce3c6f90cf0de21e6945cf8456c8d6bf5292b85a9a458d85cba66" },
                { "ur", "439db1dc1ab5b911a7775c734a363d5d921029f2aa5c2ffeab59cd332cd6991bf65770ae7e70b70f7ef1c7a0e0716e38561567dc2c71ebbea344d7e2bea92c58" },
                { "uz", "3d09809967145a72293f8630f24a603612c8ffaa5df40856459aff9e77ba73553ebb7dbeaace4cca37f84e0230bc8f62b72ec54861253395d98e59125a2b8421" },
                { "vi", "e45518255eb261034c1e22bf57ebdcea35dbeb5925b6e4456205099d9a0d086362681760b7a24e70100dee587c7744ba44ad5370a0f4d6992e61013e209f81e6" },
                { "xh", "4edf9bc117a7af7255c18b0f7c5cea5e73bb49590e60eaa9b79b8e10771e2242e518c0f7226f6b71a10b6357646303c6520f4943243d73f08101019cbe948824" },
                { "zh-CN", "f93ed37130101cbb50b0ade1cba84992cac4f39d56f59a65e31e6a94028b0433dadca0cbe37fada9b6fab30a693e68a7a7a0b8d06ac886060421c7b573c62f7f" },
                { "zh-TW", "04f4d9b770935d020089c017b5fa4d0ef629936b7a608f65f03d6b54c844e60c271313e54a2da991f9ac724d5ed1f0bb342cb694649b49f52c6ed8d747369614" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/125.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "550a205d3fb04493f26c51ddd787580756470042055764c273c08c81e1501ed2e04c6eadaf5cf3c84aa9f50beb46c0233046d64b43bbead889c6938df03bdeb9" },
                { "af", "7a2a80d27d241db6fd6598e498fe8cd3beed410393b234da463564d6945dd23453dc46267816e196e16d1469f9f96312812b66e38eb16354ca1897b50f0a3681" },
                { "an", "4992105058ab64b2d08121dcc4998f42d52468323bbac7b13ff212550c1df481bbb6bbb0ebd8b5034c012a797bdd137713aa9a63d9ccfa167d61031998e2cecb" },
                { "ar", "c045380597d9ef77d3b532a30e53b811cb53e6b498aac8ad7d2e5103152b921459ed543f967a6202679181c21a7e1ff16278558d19139046d3599e8a9b4554c1" },
                { "ast", "cf30ea6f249c9de4b7fdf4095a2c8e3096ddb0fc1959f5bcf32c440c63aa6cd1ec7578af9f9d1295ea0fde94adc0f528d49379daf0ed7e0d089630cafc6f49a7" },
                { "az", "6a3044a6a269e3a1db80617ed5b851d1fe16b0ab489d3c82be7b3ac5928442cd187dc8eb206df0f9683542e46198b9fc87db539bcafe6639cdd67182e841db56" },
                { "be", "b547615f21437fa3e7e07f6a35142f905bdc9c68d877ba0ae426162c96da4e03acf37f6ca0a08b1f2012304d079bafbe30bbd2f756d82d39851e9687f056c83c" },
                { "bg", "f1b9218a7dbddc10bdfd85619b649e44b7a208c6456a44692eaf8bea97d901e7e4c4380830df45d21100ef066fdc1e5ca7107a1df2eddf2b5aece6c11e3ebf81" },
                { "bn", "bf48b549dcf7dbe4e521b3fd3dcd0911ba55cf1f3f571169dff460d2fd8b2df371fbbae29155bfa9eebbed41380d267539224709973e956bb6e12789d5659c6e" },
                { "br", "30a4ea60df73fa9edfbcf8c39daf9ad6d8c76a06bad9a277767deb03b1fb186b8ee71b798cdd0da739f9853129e6720393487cf2875ec79cdc80732d701888ed" },
                { "bs", "6e9348a99198e78dd636374601550b2a1fdd7662f65cf21d69cd78300aff9b866e30d892b8cf2953e862bcd3d51e1f8c74043e76f5aeccfadaf5523b65baf9f3" },
                { "ca", "a679bdaed599d56653947e11cd08867ac1bea9a50744704e8e1810a55b3569c699e049eceb32fdee32c8852198e4377947bf7c9cb6997ec1c3a441498a50db45" },
                { "cak", "032454fec2323c388f5f28c58f47307f71853d560c5aaa149d8a10e3e4a02719e66d4dddb5fc6336a80384362c602661311eea7d935ebf3488c2dd14e7350178" },
                { "cs", "3071bad18b56e76fbe39f1a4681d7e37101df7482ab8f1720d275880743af0896b6b11324facfeca71aced74433f597327d7012a3ad7126d6e5dc96032c5e7c7" },
                { "cy", "f78aca8238e5c3211b169b2380559476d64826b9a6d831a6aeb510d1a2afa317ab7564a7dc13313a684e087c48cf4898246ce4c652df29555806d3d9f67826b2" },
                { "da", "ea27105788e9e4889d2ac5bab8ee9f951bc39f3a56f60363ce017ca0fb9ac2736265126d2fe7f7eb5466e3ce05bc9a0d45bcb5cc82cb3d1de9c5b4cd8cefa134" },
                { "de", "a5af3fb1e61d1e8c92dd512b44903893642e99a9b593e1b0d9f5fbe9df5521a93f663ab5e822ae04bdbbcd0979f4c024a8b63e65cfac1d373ab01daa145c06e8" },
                { "dsb", "9aa2335fb70aec2ec141a4a246e3b72153ddaec7bd854b00538261357315bad948ad0961b73928df7d1e8153a5f95534bb80c7cf474f60e77b73edee972db44f" },
                { "el", "d77586f7bd0e6a2ca993374312b2299a0a1f61eee95c51e33b89ad2c609e9f9a898554ec466abdd88d3c7045a9daad69efd414958084f3fa3304120790db5efc" },
                { "en-CA", "a650291ccd8917d1ee6faf4b135addc3dfdcf931bd700f6586ad82f603ed1286d5a20075e9f62d447ede60a6c1ec37702b331bac685bb2bd5fb75c2bc8ecfc7b" },
                { "en-GB", "0fa23910feb878afdfb1b3f2c538b89b35a1d0b86a714f77162c6646730d40d49e61056007054e9e57065ccf6aeeef7ae6feaad2ce04297006403cc15ca33464" },
                { "en-US", "a9945460b12e20d529b151c8f99bcf6afed45e061f8994aea5bb3d198b69b71a9cfb4ca43cf406d24bbb7685ca2fda1fae9536dff5487473421b5f1bed7a9aff" },
                { "eo", "41512f79adec51ae7416d8a24fb8012a907b55bd4717720d3b8d3a8d53604578fcd374e0ed672d3d42a2d4a700b15698634fab1b5d141b6ed4495bf3336798b7" },
                { "es-AR", "592b40d0fcd477b7bdbfc0b7c065a148f78402bc9b94d988d6ca5afda7e36039979badfe41f13542bd373eefd2d33849681b156e3129871e0f4c522567d2a078" },
                { "es-CL", "8a83dfaa54b1daa46e49d6fe910fe9bbecaa5759c0c34e7e0e409841bfdf9f8b78303ecd7e2abed1fd5188354c121ad023a5092ec3768848ebafe7145cf42830" },
                { "es-ES", "c467819f651437dd2d85b029a5d20c5b934cbca2e64cae18844e55aeb7c622040a01396e7c72e1fe11a8a55639e45976b6edb427d868e60cef0476024454851c" },
                { "es-MX", "6efa9b1a5f6810188dd670f7253d52531a87b60174df1d7925a57385ce42454a366598601ce9b9ea69e6dbf1e6e8d51e2cf5665fadadf0abf8e3cf88500d1a8e" },
                { "et", "150bd132156c4bbc6cf5d774a8cea4111ce012755234020dd2c26529d1feb430b90cb3ac5554952889a553cd2d0a693b08f58e7d8cb6c86e618b57f387627343" },
                { "eu", "aebe8a199caccd912bd9014f881f273c1d5d305ca58f8086b0b482e8c35bfc2f0551c81bae5810909506313de3df40b48def6406273d16f1019b77e509fbf799" },
                { "fa", "16cf3cc7fd43d8fccf9b00d6bf10c5507edf4529dbc3bb650219c448837b7a3e295f027e103d5911ee8619671a24352bdd3def21ae2a75e70c15e9ba2dd26374" },
                { "ff", "e8667aaf9703a0e78f82c7beb55a79363e2933f0f50144f041448c09e52d8fcc6a332fc4e979abd0b8c3119d4b766bd8024704a874d3012000da515ad1dcd30c" },
                { "fi", "c6befa077fb025b1888d8d4b5207c3c757187220a595009224cd0d592bfb8f3fa357574a1f9db0636c85a24ddd67bb2a83556051bccb475c704a23776f5efac4" },
                { "fr", "cd83daf1dc1a63f4505e43dcb501f639d74ee9047e8d9bfff44ecb242b44e41b46140ffbca30580ebe7b4bf9ec088c2710d59dbd46fb24e1eeb3f2f8e43dc5ba" },
                { "fur", "f4cf261775b88c7c4380765da28ae125d5eb771455418aa7259622743a744d68f8b27a60d12f6ad23f961f8dabff533f96dc13442f308c8daf40654f9e6ae14d" },
                { "fy-NL", "2cd8db00bad4304f14cfde896b97648612ca855f8c209e234bc336373a97ce9c5d14a6803c20ba6727b930ec7c29423afd7f4ef42aaa434c49eb5f8cc91e9140" },
                { "ga-IE", "b8a13dbb5f11ccf9531a468efe0f779a1b4eae97285c6467b32417bd6e30cad7fc56682860d2bf53fb18a1ae2bca0fdfa6fdb35f9b7e57d279c1d4d77abfbd5a" },
                { "gd", "d24d6a792e70c148231263b55ee08be8ce8463f2db10237297ec84d760a5b2c8a1276df4770e9f52746f60339ab2c6e5265f6cc9cbd22f66bcfc7e2e3317c883" },
                { "gl", "cb3d1fc9199e5eaabd10d88d1d3b1da5b475684309a8f5fc420f1d40c5521f4364bdfed086ba6b6c798f048172626a323f1b4939580ae5e93bd1cd174c66e812" },
                { "gn", "4b04d9225e79c29f1256943b7e678ab6a53de30b73a7914a0977c057e4f36005f2c7f71f84243097cfb690e1b425480a20fd2bf39878e2283e1c58d081c1cb25" },
                { "gu-IN", "597cfa1ca2e9c43e7d5972f07b923635cdbe8d2b5221f27e02e019e9e9269402c577b3f59575e20d6737294da5717e9e3144da5176a0ef55db1303428728c550" },
                { "he", "dca42c7eac2b438d86ed15a70062087345f299a427adf9f9e8cc5f46a5ebf579a3944c2121f0ad7c751597f5f2de6a6d8a88409eb2f0ba4bb26cb98e08cc544b" },
                { "hi-IN", "ef04c7de396e4dac079718416eefce0ee66319125e08eb98a7797d65eba149ca3207eb9940f0d0c6ac19c2506a3f86d06fc8cc11e4ed359700589f56bb30820d" },
                { "hr", "2e89eaabd82543831435d68d9b01c2a10a480dcc369c58562810eddcffa96b14f36c6e1a718302a206e00e58b8773b8ea474cd7dbf36a9322d94aa85d05df44b" },
                { "hsb", "cf140de4aa07add5dc85fe810435cfc9e68259ad085c0c6c1df9d0dce927fa7115b826bf50113cbabd4f397a68a2f0523b8d1589941c90af18243be0400cdfc1" },
                { "hu", "e0ab307fb49aad4838e0053c81ca9690852381bd1d9918ae376b2e5103092f10af5ff91b59d22a978d5e73154c7feb69bb8eaf7e73f60d1c2d7b177fdabbe0c1" },
                { "hy-AM", "e2b84ef6fc5d821cac86236899d5cb61461c411342d785d21fb7dc10780d854991d2e35e309f7f66b589bd39f31b7257539d2e7b898bda148675d5aca464d656" },
                { "ia", "6b73d867178d48c469e2d17939dbb7157d4da0478090f169c45ebb9a37984c833c68608716d59d2f715d5507ceaeaf03ae73b37b16130e4c484acc8255a5400d" },
                { "id", "7e550302e751762d9e1a289dfe9da994e1533ecbd08d7fdfea8bde5dbbf7c25f6e7a2d4111daaaab928f5a077461ffdb475b3b6f3b7ef55c4780ef45da3503e6" },
                { "is", "f7270c2c7eb7305816c9891c4ed3a23d68cd2f5372bc7485fca8b5beddcc1a6712b7e45b9b28472c23e2df879e7c66e984023470f04c83087793364e5ddc6dac" },
                { "it", "dc109eac5c3cb6e742a72a552e261adc0d17a5b34c8cb2e5617bb49334ba3208a62ba2704015aeba902e4692b471115a7b8e9bdbb3c1f52bd0deb845278cf4ad" },
                { "ja", "76198471b2118ab6abffd31b9d791bec69963ba3be1825d2d3936ebcb4fe54211a18d4eb1f59737709cdcc1d1ea6bb4bcf6e59f165a51cc970b5a2a8028e77be" },
                { "ka", "ec1b59788761b2425854e6399d591912ca844d7642942fb90411760d0b57f25f58e0d4d8f476b3d952597796f0a29c47364c1182495852272fff25b194bb7b5b" },
                { "kab", "b69101a0e1b1ff66db2edbc1b2d51ad5432a06fd3d92815d9a18e693c1ab24a72c4f852f79d8927b006561c86cb033120c34af726dc726d5d3fc685a395a70ef" },
                { "kk", "92b90dc0bdfb4abaff7e37b4269ec08c773e92a1bb86f7fefc49d8d28bc902b22e83343df51f75334f84fed860d5cf5935ec3d8ca875a3df72a41ae46f36a50f" },
                { "km", "ef35293094e996a873b1f800df7a9aa773a05f9d19e9b5981552f435b672d72f81d26a9e339ae273b4fc6e99496ba336d4689dd10398fa840d5182739afca0a3" },
                { "kn", "e97909742798491f8da0a4a63330fae90b46d03bdf65d28ad684d45206e0825503e9d6a81e2b92471194e8c8325a00d50e3c3074c2db6dba0481a091ff54a745" },
                { "ko", "ac426c8c5d5a9dd71e40e18fa8e9d641a8c1083c7cd498cccd9e32d0209724baa5319c0e7f7490f0e647c3e9347baf88edb26878258dd8413fdf6ddfbe3bdc00" },
                { "lij", "85f27307243a47c0e30fe2330d66f18d31cc7c751eea5c346139c32c10ac6bccbadb493e0f5fd28e9a0958c3a16ae4724125beb1945486134ee4d059cf152316" },
                { "lt", "f0b930762c51e1991ff0675ea058ec9e04170bad507b4a08b80fabb35fcea144d41e3c8f38a900098a7efb1a79afdd55ee103a2cfe0fe9afee6c4be908800d9c" },
                { "lv", "a1bd625b95e51c4e5bf9699aa65329430c5652e231043a93918bc57907a0fd60680b0adde2a0a0a22d3de2a004a70e6bd931e75024c82eb1767ccaa51643e668" },
                { "mk", "b18a316518f95424af0e024981df40021137e38eacee815778ed3c067dcfc6c9e1d5c4d98f4ce8083f59323c621b92a46ab983afe7f9fe5e54b6fd079e738696" },
                { "mr", "436866033bf88533ea6960193a03e7a2b90219fbd55a002a6fb5dcaed733cb473156da3ee634052e1be38b57e31b343c76c02de1ec2c8fd91658fb7ab15efb0e" },
                { "ms", "af2f2095139138b159c5824beb578269c55dcc2a50281955884b781b78b562027e03be59c511c0b25dccd110866e3cb83f68de6ad10bdd5c5d64415a13da8b69" },
                { "my", "1548ea92be94cfde1acaafee0a8fe771de219e807035cdb0919feee5210ab65a6a4a580455eff18edce8ad4f47b564ac130708bd8085c1ab8895804518a13de1" },
                { "nb-NO", "45db9e649170a6991265188f27558171bc1f9a231881c916aacb553a42a300508b3a847c7899b727e813f0e2c5b278c25315c91d70ef96929400c0dddccc8c98" },
                { "ne-NP", "3c7089babdcedee2127404d2f555ae7f95f61af4a8c13a1600ffe94421fe92c64d94a0d08e9ab769f6fa2f3e8bf5a925ee78582c9f1d4ff3b46ff096d9c5456c" },
                { "nl", "11fa85be844a211e641c2ad0ad3c4f5e27a6c3015038d14fbcb5e40d79f53df69f63e672afe70f045ef153ce89bb68362d2993ea2957d1d705fa2a2603a3616e" },
                { "nn-NO", "69f8a1cba665b18636612115dd3b23d905ab4fdcd76dd736b653aa1e748704d1c4846e914844304748d5dc0588758e1de9bd5f65091c80e845cdd584c4fbf4ba" },
                { "oc", "6a4a2cc20e6013ba977aebad70aab613cb869387fd7984497f1486e37a9c8caa35d6240204eed5b258551ab6c422140fa28872ee677a4597ea6203de5bd35efc" },
                { "pa-IN", "d744e62bb40416c76aae659456bf87f72d184b2a55165dcd18066b76981414a25e94708f036cbbf7ad754a3b833ef25c79fefab0ad9d70515916d6711dff49d9" },
                { "pl", "4246310713a70c62f735953c1659b023c53230bb081c856cb4026a80a4308a5543edfd79a577b5845414bf2593c9970054d7865fb35dcb1eb103a084918d7c7d" },
                { "pt-BR", "dcf937d2231db756a9918df43c020d3995c6ca33fa91c732b83fc7655863e80cf0a8250945b0db6d74d23975e4fcccf61ce1d1e46161899e766fdfb979e7e29d" },
                { "pt-PT", "e08a1d813fc5a5bfc703b4f011d9755fb4083cf092796695e12ee13863630c5b08b8ef350ddf920417cf80575159f17b7fd63e258c32a15cea71a0751030de27" },
                { "rm", "453a6f27bc7f4872cf86583cc5f5423899eb4904858c1e4f3de5f20c35a76bc840958a2976175b99d5a1b44813da2cffcb2ea47a759c49ba860a58855e10e85a" },
                { "ro", "c74ba910fcc697c4506c6ca733dd36c421267e3b54361a7af602d330dde7591b024377f32746b250884dfbe01236c9d760561a9baa86410b5c7543f37b1508a7" },
                { "ru", "f01a3432b249eb1358539d9adaa75bb9438c37809c0c9be809048c680144dec7465eab7e5fbdaefbb959c8d3a7314ede4de7461987fa9a4990701417de5629e4" },
                { "sat", "8a341c2700d532e90382f0b5f190f743234266bd72e4571d9c3ee4a25c0fba991cf2c6b85028da5abc24dff7e66acc972d335d1d6e2c839b7ea8c30dc6ad4131" },
                { "sc", "d0031f9f2daee223eeaa7e0d8a3f52ec1caa506a4b70e94d50d129517ed7794b0382aa17b5d30e9ef73fb2fbb88c79f898ca65ea90524918345e2fc0e3cf18d2" },
                { "sco", "8e3b9e74ab7cf54b7f691aa9bde2e1cc6c32a270204b58fe819e156587a56d23d4d30d90431d0f90af852d07bcb8da0a0d2842bc977fa8eded08a363660a4c13" },
                { "si", "f1d30a88d37704789926022e80814aa569c7b825d80be6be6fc3f784bd0622746699c6f08a5ddadb5601f167ba2e2da19500f8ab501b3879f7b0d767fa327fff" },
                { "sk", "4cfc6cb6150ac4d8aaea35bd38d4c6f43875160df28bfdc098d4dc45915fe4d2cadd1889513f6d49bdbd3a26ed971a220401591c7b45cc2f4423271c91d7150c" },
                { "sl", "5e2e2c82f3575311a0da479c177a90e60363008b4ccf8b73387bc53da238ea603f728f24130a2820f6ca567cb73fb61c89c53e04c88d63d087b10829190f1c77" },
                { "son", "a73f769fca373f898a17b8b58cab74982f387159e490054462da08b90822e68b5102470515758285204b88f93e2d54f8736fab0d953223d35a54de1b6c4bc8d5" },
                { "sq", "4468c012e17b7838f15acfaf81e075252cbf0716106ec4b13d5dd6289481657b43e5e1cebe115619c5bd22832c33fba1631499c3689d0c1a915a088ebf156807" },
                { "sr", "d2bee63e772901b5fd809c8613c553cc15a0c60835407735f0af452b73c41410a5282c34b5a482125a04f3f243113f40d2ba54de3f4c6a8628e388e4721a680d" },
                { "sv-SE", "455d98cd4a53747c2930fd23bad98a0c245fa585a222cf67ac7a6be830130307fd9c93d468282198763aa5b9972c074185238f094101e0fab8cbb7939b33deea" },
                { "szl", "a4e991efc7b5fa78f06cba496ff05bfd1f8a9956e0950fed55cdc21d96b2bb38259b498fbe65aed33bdced64f3d51a4e6a9d06eabf1aa4e7aa16bc9e29f6ecd6" },
                { "ta", "338a85d0f5c884d006161d3522a1b3b2043cb89d02dfba0cb5f14dcef80a01157e39f7fd18259b34759898fde5cb6a3aea6ce5a3a7bdc81a879295e05275be58" },
                { "te", "078792b504fc7b659670be83063f934e6234cea8ae32a95326bdaff4c0078fd1a6a67fc1ac3f9823ff32d985f3784f3e1b74a9799983b497bc32839d18fb754d" },
                { "tg", "fa0eaf6fd88380f596a9371b123b20ba3317926ed0c8548e375e2b32c8ca9debaa3a0ded5dd8ca96395e366565e217649242a38dc5b45049c60c83ca076b2f73" },
                { "th", "4eb40a5347f7be14f0f0c4448ed9ece6c7645d747fbeecbfa19f2240b123fb7e299c04d59958bc2f62b4756bfa3371cd64edb9b741f64ee281eea694ef4d16f4" },
                { "tl", "9a3022576310c6e6e291f25cff582ba419db039ed999e6000da19def31ab3325b92f86cd424ea4d2468c18c9cb786029bf668e79109c0a494abc39e400d7b40c" },
                { "tr", "6ba6f3295e480d48dba54ee3708d1aebdbf207146d3fcbdb925796e3462d792ab429e886615fc5e7eb3f63ceb76a0060ec97db14a3980d989b0eb57fb44df39a" },
                { "trs", "c0da7b32cdb8bb4638410d340536b3b6d61ddae1234a675a3b52a77febb3ff37459eddf5bba4e1352385499c70436044a241d282408bc126971418b04d2854f4" },
                { "uk", "3f23482c327086034fbba7ffc5ad31bbd50ba6c383022104a60af9c22a1c20976b4eee244ca6ce05fa1f561ff8c6b2994e170af82cd1bcabd4a0bd1fc31b83fe" },
                { "ur", "2ac257abc0efcecd70eda60534050d62774deea300ba9a14ec59f530ccbd977e78354aaa93fccaacb820b43e9d9b7551897ae32aa32e244785d90425a29c726b" },
                { "uz", "3dc235d88020b8049a48cfd1343827aa4d36c1f85c2b7e05e27fb4773bfff6aa46b4f8aabfe7ee1610bdb386ee828add0cbec49af0a2d9674b0d1247afe6e5d0" },
                { "vi", "d6cb7e7506f043e469e9993ab7070a25bef97315cafc32ff18851cacb0ef3b6f057fc8449eeee6b852b49575b5f8c6799a3f6f509276893ff2caf395f51b3d7f" },
                { "xh", "ff95f934381e45ff6759db537e68283e9c8962d8227262c61b20f5007d815e1563de245cc31bc5735174313be3c7fd357c8f109d22b0d05697abf8f4b1c68211" },
                { "zh-CN", "11948c52f749d9e85cbad53ad0dfeb4df26ddd2678443505ed5606128fb553029a3575630e561b2d5b10a3ef237c16b0940966bcb3f8b817ee309eed82708dd8" },
                { "zh-TW", "0840dbd402f7e8ef27c35f22c38adca0c57ff4e0f1280515b85c2f3296e225ff22e3a82c31a94eab7799ff4e03448a2a65b2dcb02da00cdfbbbdb4a8f3ab0f2a" }
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
            const string knownVersion = "125.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
